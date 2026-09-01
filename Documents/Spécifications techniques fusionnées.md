# Centre d'appels — GROUPE RETIF
## Spécification fusionnée (technique + UX) — Référentiel de développement du POC

| | |
|---|---|
| **Projet** | Application web « Centre d'appels » — gestion des appels sortants (outcalling) |
| **Objet du document** | Fusion des *Spécifications techniques* (v71) et de la *Spécification de l'UX* (v22) en un référentiel unique, orienté génération de pages, de code et de base de données |
| **Stack** | .NET / Blazor / PostgreSQL |
| **Données clients** | Base AS/400 en production — simulée dans le POC par la table `v001p10` (PostgreSQL) |
| **Utilisateurs** | 6 à 10 conseillers du service client siège + administrateurs |
| **Remplace** | Un fichier Excel partagé utilisé par le service client |
| **Date de fusion** | 19/08/2026 |

**Sources**
- [Spécifications techniques du projet Centre d'appels](https://retif.atlassian.net/wiki/spaces/SE/pages/2539880453/Sp+cifications+techniques+du+projet+Centre+d+appels) — v71 du 18/08/2026
- [Spécification de l'UX du projet Centre d'appels](https://retif.atlassian.net/wiki/spaces/SE/pages/2588508173/Sp+cification+de+l+UX+du+projet+Centre+d+appels) — v22 du 18/08/2026

**Historique**
| Version | Date | Contenu |
|---|---|---|
| 1.0 | 19/08/2026 | Fusion initiale des specs technique (v71) et UX (v22) |
| 1.1 | 19/08/2026 | Arbitrage des points §10.1 (déroulement « Ne plus contacter ») et §10.9 (table `TYPES_CONTACT` + colonne `ACTIONS_CAMPAGNES.IDTYPE_CONTACT`) |
| 1.2 | 19/08/2026 | Colonne « Type de contact » ajoutée à l'affichage du Suivi des campagnes (19 colonnes). Arbitrage §10.10 (pas de code campagne parlant, `IDCAMPAGNE` fait office de numéro) et §10.13 (Recherche client ouverte à toutes les campagnes pour tous les rôles) |
| 1.3 | 19/08/2026 | Arbitrage §10.3 (champ Date d'achat dans la popup) et §10.5 (historique en panneau latéral uniquement). **Report des décisions dans Confluence** : specs techniques v73, spécification UX v23 |

**Règle d'arbitrage appliquée** : en cas de divergence entre les deux documents, la spécification UX (plus récente et plus précise sur les écrans) fait foi pour l'agencement et le comportement des écrans ; la spécification technique fait foi pour le modèle de données et les règles de gestion. Toutes les divergences relevées sont listées au **§10 — Points à arbitrer**.

---

# 1. Principes directeurs

1. **Application orientée « fiche d'appel »** : minimum de friction pour un conseiller en situation d'appel téléphonique. La saisie d'une action doit être possible en quelques secondes.
2. **Un seul point de saisie** : toute saisie d'action passe par une **popup de saisie d'action unique**, commune à tous les points d'entrée. Aucune saisie directe en cellule de tableau.
3. **Tous les tableaux de consultation sont en lecture seule.**
4. **Séparation front-office / back-office** : les fonctions du quotidien du conseiller sont dans la barre d'onglets principale ; l'administration est regroupée dans une page dédiée réservée au rôle Admin.
5. **Collaboration temps réel** : polling ≤ 5 s (paramétrable) + verrous applicatifs de ligne.
6. **Traçabilité** : aucune suppression de données de campagne après activation ; journalisation de toutes les écritures.
7. **Persistance de la navigation** : barre supérieure et barre d'onglets visibles en permanence (hors page de connexion).

---

# 2. Modèle de données

## 2.1 Vue d'ensemble

```
                        ROLES
                          │ 1
                          │
                          n
                     OPERATEURS ──────────────┐
                          │ 1                 │ 1
                          │                   │
        ┌─────────────────┤ n                 │ n
        │                 │                   │
        │        CAMPAGNES_OPERATEURS         │
        │                 │ n                 │
        │                 │                   │
        │                 │ 1                 │
   E_CAMPAGNES ──1────n── L_CAMPAGNES ──1──n──┴── ACTIONS_CAMPAGNES
        │ 1                    │ 1                        │ n
        │                      │                          │
        │ n                    │ 0..1                     ├── TYPES_CONTACT
   COMS_CAMPAGNE          VERROUS_LIGNE                    ├── DEROULEMENTS
                                                           ├── INTERETS_CLIENT
                                                           ├── CANAUX_ACHAT
                                                           └── COMS_CAMPAGNE

   Tables indépendantes : CLIENTS_HORS_CONTACT, JOURNAUX_TABLES, PARAMETRES
   Source externe (lecture seule) : v001p10  ← clé (SOC, NUMCLI)
```

**Conventions retenues pour le POC**
- Noms de tables et colonnes en **majuscules dans la spec**, créés en **minuscules non quotées** dans PostgreSQL (comportement par défaut) → mapping EF Core explicite via `[Table]` / `[Column]` ou `HasDefaultSchema` + convention snake_case.
- Toute table métier porte `DHCREATION`, `DHMODIF` (`timestamptz`) et, quand applicable, `IDOPERATEUR_CM` (opérateur de création/modification).
- Clés primaires techniques auto-incrémentées (`bigint GENERATED ALWAYS AS IDENTITY`).
- Aucun `DELETE` physique sur `E_CAMPAGNES` / `L_CAMPAGNES` / `ACTIONS_CAMPAGNES` dès que la campagne est passée à *Active*.

## 2.2 Table source AS/400 simulée — `V001P10`

Lecture seule. Clé fonctionnelle **(SOC, NUMCLI)**. Alimentée dans le POC par un jeu de données fictif.

| Colonne | Type PostgreSQL | Remarque |
|---|---|---|
| SOC | `char(3)` | Code société, ex. `RET` |
| NUMCLI | `numeric(12,0)` | N° client RETIF |
| SIRET | `varchar(14)` | Facultatif |
| RAISON_SOCIALE | `varchar(120)` | |
| SOUS_ACTIVITE | `varchar(60)` | Segment métier (ex. CHR) |
| RFM | `varchar(10)` | **Donnée variable** — relue à chaque ouverture de campagne |
| CA_HT | `numeric(14,2)` | **Donnée variable** |
| DATE_DERNIER_ACHAT | `date` | **Donnée variable** |
| MAGASIN_AFFILIE | `varchar(60)` | Magasin de rattachement |
| CORRESPONDANT | `varchar(80)` | Nom du contact chez le client |
| TELEPHONE | `varchar(25)` | **Obligatoire pour l'import** |
| EMAIL | `varchar(120)` | |
| ADRESSE | `varchar(200)` | |
| CP | `varchar(10)` | |
| VILLE | `varchar(60)` | |
| PAYS | `varchar(60)` | |
| LANGUE | `varchar(20)` | |

> **Clé de conception** : les données AS/400 sont **figées** dans `L_CAMPAGNES` au moment de l'import, **sauf** `RFM`, `CA_HT` et `DATE_DERNIER_ACHAT` qui sont **relues à chaque ouverture de la campagne**. Pour le POC : un service `IClientAs400Service` avec deux méthodes — `GetSnapshotAsync(soc, numcli)` (import) et `RefreshVariablesAsync(lignes)` (ouverture de campagne).

## 2.3 Référentiel / paramètres

### `ROLES`
| Colonne | Type | Remarque |
|---|---|---|
| IDROLE | `int` PK | |
| LIBELLE | `varchar(30)` | `Conseiller`, `Admin` |
| DHCREATION / DHMODIF | `timestamptz` | |

### `OPERATEURS`
| Colonne | Type | Remarque |
|---|---|---|
| IDOPERATEUR | `bigint` PK | |
| LOGIN_AD | `varchar(50)` UNIQUE | Règle RETIF : initiale du nom + prénom simplifié, ex. `fjeanfrancois`, `spetit` |
| NOM | `varchar(60)` | |
| PRENOM | `varchar(60)` | |
| IDROLE | `int` FK → ROLES | |
| DHCREATION / DHMODIF | `timestamptz` | |

> Le mot de passe **n'est jamais stocké** : vérification via API AD. **Dans le POC, seul le login AD est saisi, sans contrôle du mot de passe.**

### `DEROULEMENTS`
| Colonne | Type |
|---|---|
| IDDEROULEMENT `int` PK / LIBELLE `varchar(60)` / DHCREATION / DHMODIF | |

Valeurs initiales : *Numéro non attribué, Faux numéro, Entreprise fermée, Mauvais interlocuteur, Doublon, Répondeur, À rappeler, Contact argumenté, **Ne plus contacter***.

> **« Ne plus contacter »** est un déroulement à part entière. Sa sélection dans la popup de saisie d'action déclenche l'insertion du couple (SOC, NUMCLI) dans `CLIENTS_HORS_CONTACT` avec `DATE_EXCLUSION = date du jour` — voir §3.3 et §3.6. C'est le seul déroulement qui écrit dans une autre table que `ACTIONS_CAMPAGNES`.

### `TYPES_CONTACT`
Canal **par lequel le contact a été réalisé** (à ne pas confondre avec `CANAUX_ACHAT`, qui désigne le canal d'*achat* en cas de vente validée).

`IDTYPE_CONTACT int PK / LIBELLE varchar(60) / DEFAUT boolean / DHCREATION / DHMODIF`

Valeurs initiales : *Appel* (défaut), *Email*, *SMS*, *Courrier*.

> La colonne `DEFAUT` permet de pré-sélectionner « Appel » dans la popup de saisie d'action : l'usage nominal étant l'appel sortant, le conseiller n'a rien à saisir dans le cas courant (principe de friction minimale, §1.1). Une seule ligne peut porter `DEFAUT = true`.

### `INTERETS_CLIENT`
`IDINTERET int PK / LIBELLE varchar(60) / DHCREATION / DHMODIF`
Valeurs initiales : *Réfractaire, Intéressé via Web, Intéressé via Mag, Vente validée*.

### `CANAUX_ACHAT`
`IDCANAL int PK / LIBELLE varchar(60) / DHCREATION / DHMODIF`
Valeurs initiales : *Web, Magasin*.

### `COMS_CAMPAGNE`
Commentaires prédéfinis, **propres à chaque campagne**.
`IDCOMMENTAIRE bigint PK / IDCAMPAGNE FK → E_CAMPAGNES / LIBELLE varchar(200) / DHCREATION / DHMODIF`

### `PARAMETRES`
`IDPARAMETRE int PK / LIBELLE varchar(60) / VALEUR_TEXTE varchar(200) / VALEUR_NUM numeric(12,2) / DHCREATION / DHMODIF`

Paramètres attendus au minimum :
| LIBELLE | Type de valeur | Défaut proposé |
|---|---|---|
| `POLLING_SECONDES` | num | 5 |
| `VERROU_EXPIRATION_MINUTES` | num | 10 |
| `ARCHIVAGE_AUTO_JOURS` | num | à définir |
| `CONSERVATION_RGPD_JOURS` | num | ~1095 (3 ans, à valider DPO) |

## 2.4 Campagnes

### `E_CAMPAGNES` — entêtes de campagne
| Colonne | Type | Remarque |
|---|---|---|
| IDCAMPAGNE | `bigint` PK | |
| NOM | `varchar(120)` | ex. « Promotions CHR juin 2026 » |
| DATE_CAMPAGNE | `date` | Date de lancement prévue |
| DESCRIPTION | `text` | Libre |
| NB_LIGNES | `int` | Dénormalisé — recalculé après chaque import/suppression de ligne |
| STATUT | `varchar(20)` | `EN_PREPARATION` \| `ACTIVE` \| `CLOTUREE` \| `ARCHIVEE` |
| DHCREATION / DHMODIF | `timestamptz` | |
| IDOPERATEUR_CM | `bigint` FK → OPERATEURS | |

### `L_CAMPAGNES` — lignes de campagne (un client dans une campagne)
| Colonne | Type | Source | Remarque |
|---|---|---|---|
| IDLCAMPAGNE | `bigint` PK | | |
| IDCAMPAGNE | `bigint` FK | | ON DELETE RESTRICT |
| NUM_LIGNE | `int` | | Séquence par campagne |
| CODE_SOC | `char(3)` | Import | |
| NUMCLI | `numeric(12,0)` | Import | |
| IDOPERATEUR_ASSIGNE | `bigint` FK, **nullable** | Admin | `NULL` = ligne libre |
| SIRET | `varchar(14)` | AS/400 — figé | |
| RAISON_SOCIALE | `varchar(120)` | AS/400 — figé | |
| SOUS_ACTIVITE | `varchar(60)` | AS/400 — figé | |
| RFM | `varchar(10)` | AS/400 — **variable** | Relu à l'ouverture |
| CA_HT | `numeric(14,2)` | AS/400 — **variable** | Relu à l'ouverture |
| DATE_DERNIER_ACHAT | `date` | AS/400 — **variable** | Relu à l'ouverture |
| MAGASIN_AFFILIE | `varchar(60)` | AS/400 — figé | |
| CORRESPONDANT | `varchar(80)` | AS/400 — figé | |
| TELEPHONE | `varchar(25)` | AS/400 — figé | |
| EMAIL | `varchar(120)` | AS/400 — figé | |
| ADRESSE | `varchar(200)` | AS/400 — figé | |
| CP | `varchar(10)` | AS/400 — figé | |
| VILLE | `varchar(60)` | AS/400 — figé | |
| PAYS | `varchar(60)` | AS/400 — figé | |
| LANGUE | `varchar(20)` | AS/400 — figé | |
| DHCREATION / DHMODIF | `timestamptz` | | |
| IDOPERATEUR_CM | `bigint` FK | | |

Contrainte recommandée : `UNIQUE (IDCAMPAGNE, CODE_SOC, NUMCLI)` — anti-doublon intra-campagne.

### `CAMPAGNES_OPERATEURS` — visibilité d'une campagne par opérateur
`IDCAMPOP bigint PK / IDCAMPAGNE FK / IDOPERATEUR FK / DHCREATION / DHMODIF`
Contrainte : `UNIQUE (IDCAMPAGNE, IDOPERATEUR)`.

> ⚠ Ne pas confondre : **`CAMPAGNES_OPERATEURS`** = qui *voit* la campagne (bouton « Affectation opérateurs ») ; **`L_CAMPAGNES.IDOPERATEUR_ASSIGNE`** = qui *traite* une ligne précise (bouton « Assigner ligne »).

## 2.5 `ACTIONS_CAMPAGNES` — cœur métier

Chaque contact (appel, email, rappel, SMS…) crée **une ligne**. Une ligne de campagne peut porter **n** actions. Chaque action référence obligatoirement une ligne de `L_CAMPAGNES`.

| Colonne | Type | Remarque |
|---|---|---|
| IDACTIONS_CAMPAGNES | `bigint` PK | |
| IDLCAMPAGNE | `bigint` FK → L_CAMPAGNES | **NOT NULL** |
| NUM_ACTION | `int` | 1, 2, 3… séquence par IDLCAMPAGNE |
| DH_ACTION | `timestamptz` | Date/heure du contact = moment de la prise de ligne par l'opérateur |
| IDOPERATEUR | `bigint` FK | Opérateur ayant réalisé l'action (pré-rempli avec l'opérateur connecté) |
| IDTYPE_CONTACT | `int` FK → TYPES_CONTACT | **NOT NULL** — canal du contact (Appel, Email, SMS, Courrier). Pré-rempli avec la valeur par défaut |
| IDDEROULEMENT | `int` FK → DEROULEMENTS | |
| IDINTERET | `int` FK → INTERETS_CLIENT, nullable | Conditionné à Déroulement = *Contact argumenté* |
| DATE_RELANCE | `date` nullable | Conditionné à Déroulement = *À rappeler* |
| DATE_ACHAT | `date` nullable | Conditionné à Intérêt = *Vente validée* |
| IDCANAL | `int` FK → CANAUX_ACHAT, nullable | Conditionné à Intérêt = *Vente validée* |
| IDCOMMENTAIRE | `bigint` FK → COMS_CAMPAGNE, nullable | Liste de choix propre à la campagne |
| COMMENTAIRE_LIBRE | `text` nullable | Saisie libre |
| DHCREATION / DHMODIF | `timestamptz` | |
| IDOPERATEUR_CM | `bigint` FK | Auteur de la création/modification |

Contrainte recommandée : `UNIQUE (IDLCAMPAGNE, NUM_ACTION)`.

**Notion de « dernière action »** — utilisée par tous les écrans de consultation :
```sql
-- Dernière action par ligne de campagne
SELECT DISTINCT ON (a.idlcampagne) a.*
FROM   actions_campagnes a
ORDER  BY a.idlcampagne, a.num_action DESC;
```
→ Encapsuler dans une **vue** `V_DERNIERE_ACTION` pour être réutilisée par Suivi des campagnes, Rappels du jour, Recherche client et Tableau de bord.

## 2.6 Verrous, opposition, journalisation

### `VERROUS_LIGNE`
| Colonne | Type | Remarque |
|---|---|---|
| IDLCAMPAGNE | `bigint` PK, FK | Une ligne ne peut porter qu'un verrou → PK naturelle |
| IDOPERATEUR | `bigint` FK | Détenteur |
| DH_VERROU | `timestamptz` | Renouvelé par heartbeat |

### `CLIENTS_HORS_CONTACT`
`IDCLIENTSHC bigint PK / SOC char(3) / NUMCLI numeric(12,0) / DATE_EXCLUSION date`
Contrainte : `UNIQUE (SOC, NUMCLI)`. `SOC + NUMCLI` identifie formellement un client — ni SIRET ni téléphone nécessaires.

### `JOURNAUX_TABLES`
`IDJOURNAL bigint PK / UTILISATEUR varchar(50) / DHCREATION timestamptz / NOM_TABLE varchar(60) / IDTABLE bigint / CHAMP_TABLE varchar(60) / ANCIENNE_VAL text / NOUVELLE_VAL text`

Périmètre : **écritures uniquement**, pas de traçage des lectures. Sert aux litiges internes, aux demandes RGPD, et rend contrôlable la règle « modifiable le jour même par l'auteur, ensuite par l'admin ».
→ Implémentation POC : intercepteur EF Core `SaveChangesInterceptor` comparant `EntityEntry.OriginalValues` / `CurrentValues`.

---

# 3. Règles de gestion transverses

## 3.1 Cycle de vie d'une campagne

| Statut | Visible conseillers | Modifiable | Actions saisissables | Transitions autorisées |
|---|---|---|---|---|
| **EN_PREPARATION** | ❌ | ✔ (entête + lignes + import + suppression) | ❌ | → ACTIVE |
| **ACTIVE** | ✔ (selon visibilité) | Entête : non ; lignes : non | ✔ | → CLOTUREE |
| **CLOTUREE** | ✔ lecture seule | ❌ | ❌ | → ARCHIVEE |
| **ARCHIVEE** | ❌ (conservation) | ❌ | ❌ | — (purge/anonymisation RGPD) |

- Aucune suppression de données dans les tables de campagne dès le passage à *Active* — seul le statut évolue.
- Passage en *Archivée* : à la demande, ou automatique après une période paramétrable (`ARCHIVAGE_AUTO_JOURS`).
- **RGPD** : purge ou anonymisation à l'échéance (conservation des compteurs statistiques, effacement des colonnes nominatives). Durée exacte et mécanisme (tâche planifiée vs revue manuelle annuelle) **à définir avec le DPO avant mise en production** — référence CNIL prospection B2B : ordre de 3 ans après dernier contact.

## 3.2 Chaîne de contrôles à l'import

Appliquée ligne par ligne, alimente la colonne **Validation** :

| Ordre | Contrôle | Message dans Validation | Effaçable manuellement |
|---|---|---|---|
| 1 | Format du fichier (2 colonnes : `char(3)` + numérique 12) | Refus global du fichier, message d'erreur explicite | — |
| 2 | Code société incorrect | `Soc incorrect` | ❌ |
| 3 | Couple Soc/Numcli inexistant dans `V001P10` | `Numcli inconnu` | ❌ |
| 4 | Téléphone absent | `Téléphone absent` | ❌ |
| 5 | Doublon dans le fichier importé | `Doublon` | ❌ |
| 6 | Client déjà présent dans une autre campagne | `Déjà présent dans la campagne XXX du JJ/MM/AAAA` | ✔ |
| 7 | Client présent dans `CLIENTS_HORS_CONTACT` | `Client hors contact` | ❌ (voir §10.2) |

Seules les lignes dont **Validation est vide** sont écrites dans `L_CAMPAGNES` par le bouton *Charger les lignes dans la campagne*.

## 3.3 Dépendances conditionnelles de saisie (popup d'action)

```
Déroulement = « Contact argumenté »  →  Intérêt du client   saisissable (sinon grisé)
Déroulement = « À rappeler »         →  Date de relance     saisissable (sinon grisée)
Intérêt     = « Vente validée »      →  Date d'achat        saisissable (sinon grisée)
                                     →  Canal d'achat       saisissable (sinon grisé)
Déroulement = « Ne plus contacter »  →  Intérêt / Date de relance / Date d'achat / Canal
                                         forcés à vide et grisés
                                     →  à l'enregistrement : INSERT dans
                                         CLIENTS_HORS_CONTACT (SOC, NUMCLI, today)
                                         si le couple n'y figure pas déjà
```
- Le champ **Type de contact** est toujours saisissable, indépendamment du déroulement, et pré-rempli avec la valeur par défaut (`Appel`).
- Un changement de Déroulement ou d'Intérêt **remet à vide** les champs devenus non saisissables.
- Le choix « Ne plus contacter » doit faire l'objet d'une **confirmation explicite** avant enregistrement (« Ce client sera exclu de toutes les campagnes futures. Confirmer ? »), l'opération étant réversible uniquement par l'admin.

## 3.4 Exclusivité d'écriture

- L'exclusivité porte sur **chaque action**, pas sur la ligne de campagne.
- Une action n'est modifiable que par **son auteur, le jour même**, et par **l'admin sans limite de temps**.
- Un autre conseiller peut à tout moment créer une **nouvelle action** sur la même ligne de campagne.

## 3.5 Verrous de ligne et temps réel

**Polling** : le navigateur interroge le serveur toutes N secondes (`POLLING_SECONDES`, défaut 5) pour récupérer les lignes modifiées depuis son dernier passage (horodatage de dernière modification). Suffisant pour 5 à 10 conseillers simultanés.

**Verrou applicatif** :
- Acquisition à l'ouverture de la ligne (= ouverture de la popup de saisie d'action).
- Libération à l'enregistrement, à la fermeture de la popup, ou au changement de ligne.
- **Expiration automatique** après `VERROU_EXPIRATION_MINUTES` (défaut 10) d'inactivité, renouvelée par **heartbeat** tant que la page est active — couvre le navigateur fermé brutalement.
- Verrou **visible** des autres conseillers : ligne marquée « en cours — Prénom N. », non ouvrable.
- L'admin peut libérer un verrou manuellement (Administration → Paramètres → Verrous actifs).

## 3.6 Liste d'opposition

- Alimentée automatiquement par le choix « Ne plus contacter » saisi en appel, ou manuellement par l'admin (demande reçue par un autre canal).
- Consultée à chaque import.
- Un refus exprimé dans la campagne 26_003 doit empêcher un rappel par la 26_017 six mois plus tard, tant que le client est dans la table.
- Un client qui change d'avis est **retiré** de la table.

## 3.7 Algorithme « Prochain contact »

Recherche du **premier** client tel que :
1. **Non traité** → aucune ligne dans `ACTIONS_CAMPAGNES` pour cette `IDLCAMPAGNE` ;
2. **Non verrouillé** → absent de `VERROUS_LIGNE` (ou verrou expiré) ;
3. **Assigné à moi ou à personne** → `IDOPERATEUR_ASSIGNE IS NULL OR IDOPERATEUR_ASSIGNE = @moi`.

Les deux mécanismes (attribution automatique et plages assignées par l'admin) **coexistent** : le prochain contact doit respecter les plages assignées.

```sql
SELECT l.*
FROM   l_campagnes l
JOIN   e_campagnes e ON e.idcampagne = l.idcampagne
LEFT   JOIN actions_campagnes a ON a.idlcampagne = l.idlcampagne
LEFT   JOIN verrous_ligne v ON v.idlcampagne = l.idlcampagne
                            AND v.dh_verrou > now() - (@expirationMinutes * interval '1 minute')
WHERE  l.idcampagne = @idCampagne
  AND  e.statut = 'ACTIVE'
  AND  a.idlcampagne IS NULL          -- non traité
  AND  v.idlcampagne IS NULL          -- non verrouillé
  AND  (l.idoperateur_assigne IS NULL OR l.idoperateur_assigne = @idOperateur)
ORDER  BY l.num_ligne
LIMIT  1
FOR UPDATE OF l SKIP LOCKED;          -- évite l'attribution concurrente
```

## 3.8 Matrice des droits

| Capacité | Conseiller | Admin |
|---|:--:|:--:|
| Voir les campagnes actives (selon visibilité définie) | ✔ | ✔ |
| Saisir / modifier ses propres tentatives (jour même) | ✔ | ✔ |
| Modifier toute tentative, à tout moment | | ✔ |
| Créer une campagne, importer, configurer | | ✔ |
| Clôturer / archiver une campagne, libérer un verrou | | ✔ |
| Exporter les données | | ✔ |
| Gérer la liste d'opposition et les comptes | | ✔ |
| Gérer les tables de paramètres | | ✔ |
| Impressions PDF | | ✔ |

Le rôle est identifié au login et conservé en variable globale de session, testée avant chaque action. Les droits sont gérés **par programmation** dans les pages et traitements (pas de système de permissions générique).

---

# 4. Architecture des écrans et navigation

```
┌─────────────────────────────────────────────────────────────────────────┐
│  Centre d'appels                        [Aide]  spetit  [Déconnexion]   │  ← barre supérieure persistante
├─────────────────────────────────────────────────────────────────────────┤
│ Suivi des campagnes │ Rappels du jour ⑶ │ Recherche client │ Tableau de │
│                     │                   │                  │ bord │ Admin│  ← barre d'onglets persistante
└─────────────────────────────────────────────────────────────────────────┘
                                                                    ▲
                                                     visible si rôle = Admin
```

## 4.1 Arborescence / routes Blazor proposées

| Écran | Route | Rôle requis |
|---|---|---|
| Connexion | `/login` | — |
| Suivi des campagnes (page d'accueil) | `/` ou `/suivi` | Conseiller, Admin |
| Rappels du jour | `/rappels` | Conseiller, Admin |
| Recherche client | `/recherche` | Conseiller, Admin |
| Tableau de bord | `/tableau-de-bord` | Conseiller, Admin |
| Administration → Gestion des campagnes | `/admin/campagnes` | Admin |
| Administration → Paramètres | `/admin/parametres` | Admin |
| Administration → Exports & éditions | `/admin/exports` | Admin |

## 4.2 Barre supérieure (persistante)

- À gauche : titre « Centre d'appels ».
- À droite dans l'ordre : bouton **Aide** (aide en ligne — phase ultérieure), **login de l'opérateur connecté**, lien **Déconnexion**.
- L'affichage du login est important sur les postes partagés : il permet de vérifier d'un coup d'œil sous quelle identité on est connecté, cette identité pré-remplissant la colonne « Opérateur » lors de la saisie des actions (exemple : `spetit` pour Sophie Petit).

## 4.3 Barre d'onglets principale

1. **Suivi des campagnes** — traitement des appels (remplace le fichier Excel partagé).
2. **Rappels du jour** — avec **badge compteur permanent**, visible depuis tous les écrans.
3. **Recherche client** — recherche transverse (cas du client qui rappelle).
4. **Tableau de bord** — chiffres clés par campagne.
5. **Administration** — visible uniquement pour le rôle Admin ; ouvre une page à sous-navigation propre.

## 4.4 Composants partagés à développer

| Composant | Utilisé par |
|---|---|
| `PopupSaisieAction` | Suivi (Prochain contact / Relance / Historique), Rappels du jour, Recherche client |
| `TableLignesCampagne` (lecture seule, surlignage, badge verrou, polling) | Suivi des campagnes |
| `TableResultatsClient` (colonnes communes + bouton Traiter) | Rappels du jour, Recherche client |
| `ZoneSaisieBasDePage` (Valider / Annuler) | Admin campagnes (entêtes + lignes), Paramètres |
| `BadgeCompteurRappels` | Barre d'onglets (toutes pages) |
| `ServiceVerrous` (acquisition, heartbeat, libération, expiration) | Popup de saisie d'action |
| `ServicePolling` | Suivi des campagnes, badge compteur |

---

# 5. Écrans front-office (conseiller)

## 5.1 Page de connexion — `/login`

Première page affichée au lancement. Contient :
- Le titre « Centre d'appels ».
- La saisie du **login AD** et du **mot de passe**.

**POC** : seul le login AD est saisi, sans contrôle du mot de passe. Le login est recherché dans `OPERATEURS` ; son `IDROLE` détermine la visibilité de l'onglet Administration.

## 5.2 Écran « Suivi des campagnes » — `/suivi`

Écran principal, utilisé en situation d'appel. Composé de deux tables superposées et d'une barre d'actions.

### Table haute — mes campagnes en cours
Quelques lignes listant les campagnes **actives** visibles par l'opérateur connecté (jointure `CAMPAGNES_OPERATEURS`). La sélection d'une campagne alimente la table principale.
→ À l'ouverture d'une campagne : **relire `RFM`, `CA_HT`, `DATE_DERNIER_ACHAT`** depuis `V001P10`.

### Table principale — lignes de la campagne

Colonnes, dans l'ordre :

| # | Colonne | Origine |
|---|---|---|
| 1 | Soc | `L_CAMPAGNES.CODE_SOC` |
| 2 | Numcli | `L_CAMPAGNES.NUMCLI` |
| 3 | RFM | `L_CAMPAGNES.RFM` (relu) |
| 4 | Raison sociale | `L_CAMPAGNES.RAISON_SOCIALE` |
| 5 | Sous-activité | `L_CAMPAGNES.SOUS_ACTIVITE` |
| 6 | CA HT | `L_CAMPAGNES.CA_HT` (relu) |
| 7 | Date dernier achat | `L_CAMPAGNES.DATE_DERNIER_ACHAT` (relu) |
| 8 | Correspondant | `L_CAMPAGNES.CORRESPONDANT` |
| 9 | Téléphone | `L_CAMPAGNES.TELEPHONE` |
| 10 | Email | `L_CAMPAGNES.EMAIL` |
| 11 | Magasin de rattachement | `L_CAMPAGNES.MAGASIN_AFFILIE` |
| 12 | Date/heure de contact | `V_DERNIERE_ACTION.DH_ACTION` |
| 13 | **Type de contact** | `V_DERNIERE_ACTION` → `TYPES_CONTACT.LIBELLE` |
| 14 | Déroulement | `V_DERNIERE_ACTION` → `DEROULEMENTS.LIBELLE` |
| 15 | Date de relance | `V_DERNIERE_ACTION.DATE_RELANCE` |
| 16 | Intérêt du client | `V_DERNIERE_ACTION` → `INTERETS_CLIENT.LIBELLE` |
| 17 | Canal d'achat | `V_DERNIERE_ACTION` → `CANAUX_ACHAT.LIBELLE` |
| 18 | Commentaire | `V_DERNIERE_ACTION` → `COMS_CAMPAGNE.LIBELLE` et/ou `COMMENTAIRE_LIBRE` |
| 19 | Opérateur en cours | `V_DERNIERE_ACTION` → `OPERATEURS` ; par défaut l'opérateur connecté |

> **19 colonnes** : c'est beaucoup pour un écran utilisé en situation d'appel. Le POC les affiche toutes afin de valider en pratique lesquelles sont réellement consultées ; l'arbitrage définitif se fera après retour des conseillers. Recommandation de développement : implémenter la liste de colonnes comme une **configuration déclarative** (tableau de définitions `{ Cle, Libelle, Largeur, Visible, Ordre }`) plutôt qu'en dur dans le markup, afin qu'un masquage ultérieur ne coûte qu'un changement de valeur. Un sélecteur de colonnes visible par l'utilisateur pourra être ajouté sans refonte.

Comportements :
- **Lecture seule** — aucune saisie en cellule. Rendu « table unique » pour l'utilisateur, alors que les données proviennent de `L_CAMPAGNES` + dernière action.
- **Focus sur la ligne en cours** : surlignage de la ligne en cours de traitement. Traitement **purement frontal**, sans lien avec la base.
- **Verrou visible** : ligne prise par un autre opérateur signalée « en cours — Prénom N. », non ouvrable.
- **Rafraîchissement temps réel** par polling ≤ 5 s pour refléter les actions des autres conseillers.

### Barre d'actions (au-dessus de la table principale)

| Bouton | Sélection préalable | Effet |
|---|---|---|
| **Prochain contact** | non | Attribue le premier client non traité et non verrouillé (§3.7), puis ouvre la popup de saisie d'action |
| **Relance** | ligne | Crée une **nouvelle action** (nouvelle tentative) sur la ligne et ouvre la popup |
| **Historique** | ligne | Ouvre la popup de saisie d'action, qui contient le panneau d'historique des actions précédentes |

## 5.3 Popup de saisie d'action — composant central

Fenêtre **commune** aux cinq points d'entrée : Prochain contact, Relance, Historique, Rappels du jour (bouton Traiter), Recherche client (bouton Traiter).

**Zone 1 — Rappel des informations client** (haut, lecture seule)
Raison sociale, Soc/Numcli, Téléphone, RFM, Magasin de rattachement, Correspondant. Objectif : situer l'interlocuteur d'un coup d'œil.

**Zone 2 — Champs de saisie de l'action**
| Champ | Type | Condition d'activation |
|---|---|---|
| Type de contact | liste (`TYPES_CONTACT`) | toujours — **pré-rempli à « Appel »** |
| Déroulement | liste (`DEROULEMENTS`) | toujours |
| Intérêt du client | liste (`INTERETS_CLIENT`) | Déroulement = *Contact argumenté* |
| Date de relance | date | Déroulement = *À rappeler* |
| Date d'achat | date | Intérêt = *Vente validée* |
| Canal d'achat | liste (`CANAUX_ACHAT`) | Intérêt = *Vente validée* |
| Commentaire (liste) | liste (`COMS_CAMPAGNE` de la campagne) | toujours |
| Commentaire libre | texte | toujours |

**Zone 3 — Panneau latéral d'historique** (droite)
Liste des actions précédentes de la ligne : date, opérateur, déroulement — **les plus récentes en premier**. Répond à l'exigence « historique visible sans pollution ».

**Cycle de verrou** : l'ouverture de la ligne **acquiert** le verrou ; l'enregistrement ou la fermeture le **libère**.

**À l'enregistrement** : création d'une ligne `ACTIONS_CAMPAGNES` avec `NUM_ACTION = MAX+1`, `DH_ACTION` = moment de la prise de ligne, `IDOPERATEUR` = opérateur connecté, `IDTYPE_CONTACT` = valeur choisie (défaut `Appel`). Si `Déroulement = « Ne plus contacter »`, insertion complémentaire dans `CLIENTS_HORS_CONTACT` dans la **même transaction**.

## 5.4 Écran « Rappels du jour » — `/rappels`

Page dédiée, **distincte de la Gestion des campagnes**. Évite qu'un rappel noyé dans une campagne de plusieurs centaines de lignes soit oublié.

### Compteur permanent
Badge sur l'onglet « Rappels du jour », **toujours visible** depuis n'importe quel écran — pousse l'information vers l'opérateur plutôt que de l'obliger à aller la chercher. Rafraîchi par polling.

### Portée
Compteur **et** liste affichent **tous les rappels dus dans les campagnes visibles par l'opérateur**, **quel que soit le conseiller ayant planifié le rappel** : une relance peut être prise par un autre conseiller que celui qui l'a fixée.

### Critère de sélection
Clients dont la **dernière action** a `Déroulement = « À rappeler »` **et** `DATE_RELANCE <= aujourd'hui`.

```sql
SELECT e.nom AS campagne, l.raison_sociale, l.telephone,
       d.date_relance, der.libelle AS derniere_action
FROM   v_derniere_action d
JOIN   l_campagnes l  ON l.idlcampagne = d.idlcampagne
JOIN   e_campagnes e  ON e.idcampagne  = l.idcampagne
JOIN   deroulements der ON der.idderoulement = d.idderoulement
JOIN   campagnes_operateurs co ON co.idcampagne = e.idcampagne
                              AND co.idoperateur = @idOperateur
WHERE  e.statut = 'ACTIVE'
  AND  der.libelle = 'À rappeler'
  AND  d.date_relance <= current_date
ORDER  BY d.date_relance;
```

### Liste
Colonnes : **Nom de la campagne, Raison sociale, Téléphone, Date/heure de relance, Dernière action**.
Bouton **Traiter** en bout de ligne → ouvre la ligne de campagne (acquisition du verrou incluse) dans la popup de saisie d'action, sans avoir à retrouver manuellement la campagne d'origine.

## 5.5 Écran « Recherche client » — `/recherche`

Page dédiée, accessible depuis toutes les pages. Traite le cas du **client qui rappelle de lui-même**, indépendamment de la campagne sur laquelle l'opérateur travaille.

### Barre de recherche
À l'ouverture : barre de recherche et **aucun résultat**. Recherche par **nom, NUMCLI ou téléphone**. Les résultats ne s'affichent qu'après saisie et lancement de la recherche.

### Portée élargie — **tous les rôles**
La recherche porte sur **toutes les campagnes**, actives comme clôturées, **y compris celles qui ne sont pas visibles par l'opérateur** dans le reste de l'application. Cette portée élargie s'applique **aux conseillers comme aux administrateurs** (décision du 19/08/2026, cf. §10.13).

Justification métier : un client qui rappelle de lui-même tombe sur le premier conseiller disponible, qui n'a aucune raison d'avoir accès à la campagne d'origine. Restreindre la portée obligerait à transférer l'appel, ce qui va à l'encontre de l'objectif de friction minimale. La fonction sert par ailleurs à répondre à une **demande d'accès RGPD** (retrouver toutes les données d'un client).

Contreparties retenues :
- La recherche **exige une saisie** : aucun résultat n'est affiché à l'ouverture de la page, et aucune liste exhaustive n'est accessible. La fonction permet de retrouver *un* client connu, pas de parcourir la base.
- L'accès reste limité aux opérateurs authentifiés, et l'affichage se limite aux 5 colonnes de résultat (pas d'adresse, pas d'email, pas de CA).
- Voir **§10.13** pour la limite assumée sur la traçabilité.

### Liste des résultats
Mêmes colonnes que les Rappels du jour : Nom de la campagne, Raison sociale, Téléphone, Date/heure de relance, Dernière action.
- **Date/heure de relance** renseignée **uniquement** si le client a un rappel en cours ; sinon vide.
- Un même client peut apparaître sur **plusieurs lignes**, une par campagne où il figure (y compris campagnes clôturées).
- Campagne **Active** → bouton **Traiter** (verrou + popup de saisie d'action).
- Campagne **Clôturée** → ligne en lecture seule, mention « campagne clôturée » à la place du bouton. Permet de consulter l'historique d'un client sans modifier une campagne terminée.

## 5.6 Écran « Tableau de bord » — `/tableau-de-bord`

Page de synthèse **volontairement simple**. Le suivi détaillé des campagnes dans le temps n'est **pas** assuré par cet écran : il relève d'un **rapport Power BI dédié**, alimenté par les exports.

### Sélecteur de campagne
Les chiffres sont affichés **par campagne**. Sélecteur en haut de page parmi les campagnes visibles par l'opérateur ; le reste de la page se met à jour selon ce choix.

### Barre de progression et compteurs

| Indicateur | Définition |
|---|---|
| **Barre de progression** | part de contacts traités sur le total importé (`NB_LIGNES`) |
| **Traités** | lignes ayant au moins une action |
| **Restants** | `NB_LIGNES` − Traités |
| **À rappeler** | lignes dont la dernière action a Déroulement = *À rappeler* |
| **Taux d'argumentation** | contacts *Contact argumenté* / contacts traités |
| **Ventes validées** | lignes dont la dernière action a Intérêt = *Vente validée* |

---

# 6. Écrans back-office (Administration)

Onglet visible uniquement pour le rôle Admin ; ouvre une page disposant d'une **seconde barre d'onglets** : Gestion des campagnes / Paramètres / Exports & éditions.

## 6.1 Gestion des campagnes — `/admin/campagnes`

Organisé en **deux onglets internes**.

### Onglet « Entêtes de campagne »

Table centrale (`E_CAMPAGNES`) : **Nom, Date campagne, Description, Nb lignes, Statut**.
Zone de saisie en bas de page pour afficher/éditer les champs de la campagne sélectionnée, avec boutons **Valider** / **Annuler**.

Barre de **six boutons** au-dessus de la table :

| Bouton | Disponibilité | Effet |
|---|---|---|
| **Créer** | toujours | Affiche les champs de saisie d'une nouvelle campagne en bas de page ; confirmation par Valider ; la campagne apparaît dans la table |
| **Modifier** | statut *En préparation* | Édition de l'entête via la zone de bas de page |
| **Supprimer** | statut *En préparation* | Suppression de la campagne |
| **Changer le statut** | selon statut | En préparation → Active → Clôturée → Archivée |
| **Import Excel** | statut *En préparation* | Ouvre la popup d'import |
| **Affectation opérateurs** | toujours | Liste des opérateurs avec cases à cocher : définit **quels opérateurs voient la campagne** (`CAMPAGNES_OPERATEURS`) |

> ⚠ « Affectation opérateurs » (visibilité de la campagne) ≠ « Assigner ligne » (réservation de lignes précises).

### Popup d'import Excel / CSV

1. Zone de choix de fichier (`.xlsx` / `.csv`) ; le chemin s'affiche dans un champ.
2. Bouton **Importer** → lecture du fichier et affichage dans une table : **Soc, Numcli, Téléphone, Email, Adresse, CP, Ville, Validation**.
3. Application de la chaîne de contrôles (§3.2). Le motif « Déjà présent dans la campagne XXX du JJ/MM/AAAA » peut être **effacé manuellement** pour forcer le chargement.
4. Bouton **Charger les lignes dans la campagne** → écrit dans `L_CAMPAGNES` **uniquement** les lignes dont Validation est vide, puis met à jour `E_CAMPAGNES.NB_LIGNES`.

**Format d'import** : fixé dès le départ, **deux colonnes uniquement** — code société `char(3)` et n° client RETIF numérique 12. Exemple : `RET, 20008475`. Un fichier non conforme est refusé avec un message d'erreur explicite.

### Onglet « Lignes de campagne »

Affiche les lignes (`L_CAMPAGNES`) de la campagne sélectionnée dans l'onglet précédent.

| Bouton | Disponibilité | Effet |
|---|---|---|
| **Modifier** | statut *En préparation* | Édition via zone de saisie en bas de page |
| **Supprimer** | statut *En préparation* | Suppression de la ligne |
| **Assigner ligne** | — | Réserve les lignes **sélectionnées** (sélection multiple) à un opérateur précis → `IDOPERATEUR_ASSIGNE` |

Une ligne sans opérateur assigné (affichée « — ») reste libre et attribuable via « Prochain contact ».
Cas d'usage de l'assignation : répartir une campagne géographiquement, ou réserver une partie à un conseiller qui connaît bien certains types de clients.

## 6.2 Paramètres — `/admin/parametres`

Organisation : **menu de rubriques à gauche** ; la sélection affiche à droite la table correspondante et une zone d'édition. Deux groupes de rubriques.

### Groupe « Listes de valeurs »
Rubriques éditables comme de simples listes de libellés (ajout, modification, suppression) avec boutons Valider / Annuler :

| Rubrique | Table | Particularité |
|---|---|---|
| Opérateurs | `OPERATEURS` | login AD, nom, prénom, rôle |
| Types de contact | `TYPES_CONTACT` | Appel, Email, SMS, Courrier — une seule valeur peut être cochée « par défaut » |
| Déroulements | `DEROULEMENTS` | Inclut « Ne plus contacter » ; ce libellé ne doit pas être supprimable (règle métier câblée) |
| Intérêts client | `INTERETS_CLIENT` | |
| Canaux d'achat | `CANAUX_ACHAT` | |
| Commentaires par campagne | `COMS_CAMPAGNE` | **Sélecteur de campagne** d'abord, puis liste des commentaires de cette campagne |
| Paramètres divers | `PARAMETRES` | durée du polling, jours avant archivage automatique, etc. |

### Groupe « Données »
Rubriques de gestion de données transverses (pas de simples listes) :

| Rubrique | Table | Actions |
|---|---|---|
| Clients hors contact | `CLIENTS_HORS_CONTACT` | Consulter (SOC, NUMCLI, date d'exclusion), **ajouter** manuellement, **retirer** un client qui change d'avis |
| Verrous actifs | `VERROUS_LIGNE` | Supervision (ligne, opérateur, date du verrou) + action **Libérer** pour débloquer un verrou coincé |

## 6.3 Exports & éditions — `/admin/exports`

Deux blocs distincts.

### Bloc « Export Excel (BI) »
Génère un `.xlsx` destiné à alimenter la base BI de suivi des actions Outcalling. L'admin sélectionne **une ou plusieurs campagnes** (cases à cocher), puis lance l'export.

Colonnes exportées :
| Colonne | Contenu |
|---|---|
| Prestataire | valeur fixe **« Siège SC »** |
| Soc | `L_CAMPAGNES.CODE_SOC` |
| Numcli | `L_CAMPAGNES.NUMCLI` |
| SIRET | `L_CAMPAGNES.SIRET` — **facultatif** |
| Date de campagne | `E_CAMPAGNES.DATE_CAMPAGNE` |
| Nom de l'opérateur | `OPERATEURS.NOM` (via l'action) |
| Commentaire | **le nom de la campagne** (`E_CAMPAGNES.NOM`) |

### Bloc « Édition PDF (état de campagne) »
État de synthèse imprimable pour **une seule campagne** (liste déroulante), déclenché par le bouton **Imprimer l'état**. État **agrégé**, sans coordonnées client — **à l'exception de la liste des rappels planifiés**.

Contenu : en-tête d'identification (nom de la campagne et numéro = `IDCAMPAGNE`, période, date et heure d'édition, éditeur), synthèse chiffrée, répartition par Déroulement, répartition par **Type de contact**, répartition par Intérêt client, ventes validées, rappels en attente.

---

# 7. DDL PostgreSQL de référence (POC)

```sql
-- ============================================================
-- Source AS/400 simulée
-- ============================================================
CREATE TABLE v001p10 (
  soc                 char(3)         NOT NULL,
  numcli              numeric(12,0)   NOT NULL,
  siret               varchar(14),
  raison_sociale      varchar(120),
  sous_activite       varchar(60),
  rfm                 varchar(10),
  ca_ht               numeric(14,2),
  date_dernier_achat  date,
  magasin_affilie     varchar(60),
  correspondant       varchar(80),
  telephone           varchar(25),
  email               varchar(120),
  adresse             varchar(200),
  cp                  varchar(10),
  ville               varchar(60),
  pays                varchar(60),
  langue              varchar(20),
  CONSTRAINT pk_v001p10 PRIMARY KEY (soc, numcli)
);

-- ============================================================
-- Référentiel
-- ============================================================
CREATE TABLE roles (
  idrole      int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  libelle     varchar(30) NOT NULL UNIQUE,
  dhcreation  timestamptz NOT NULL DEFAULT now(),
  dhmodif     timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE operateurs (
  idoperateur bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  login_ad    varchar(50) NOT NULL UNIQUE,
  nom         varchar(60) NOT NULL,
  prenom      varchar(60) NOT NULL,
  idrole      int NOT NULL REFERENCES roles(idrole),
  dhcreation  timestamptz NOT NULL DEFAULT now(),
  dhmodif     timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE types_contact (
  idtype_contact int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  libelle        varchar(60) NOT NULL UNIQUE,
  defaut         boolean     NOT NULL DEFAULT false,
  dhcreation     timestamptz NOT NULL DEFAULT now(),
  dhmodif        timestamptz NOT NULL DEFAULT now()
);
-- Une seule valeur par défaut possible
CREATE UNIQUE INDEX uq_types_contact_defaut
  ON types_contact(defaut) WHERE defaut = true;

CREATE TABLE deroulements (
  idderoulement int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  libelle       varchar(60) NOT NULL UNIQUE,
  dhcreation    timestamptz NOT NULL DEFAULT now(),
  dhmodif       timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE interets_client (
  idinteret  int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  libelle    varchar(60) NOT NULL UNIQUE,
  dhcreation timestamptz NOT NULL DEFAULT now(),
  dhmodif    timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE canaux_achat (
  idcanal    int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  libelle    varchar(60) NOT NULL UNIQUE,
  dhcreation timestamptz NOT NULL DEFAULT now(),
  dhmodif    timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE parametres (
  idparametre  int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  libelle      varchar(60) NOT NULL UNIQUE,
  valeur_texte varchar(200),
  valeur_num   numeric(12,2),
  dhcreation   timestamptz NOT NULL DEFAULT now(),
  dhmodif      timestamptz NOT NULL DEFAULT now()
);

-- ============================================================
-- Campagnes
-- ============================================================
CREATE TABLE e_campagnes (
  idcampagne      bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  nom             varchar(120) NOT NULL,
  date_campagne   date         NOT NULL,
  description     text,
  nb_lignes       int          NOT NULL DEFAULT 0,
  statut          varchar(20)  NOT NULL DEFAULT 'EN_PREPARATION'
                  CHECK (statut IN ('EN_PREPARATION','ACTIVE','CLOTUREE','ARCHIVEE')),
  dhcreation      timestamptz  NOT NULL DEFAULT now(),
  dhmodif         timestamptz  NOT NULL DEFAULT now(),
  idoperateur_cm  bigint REFERENCES operateurs(idoperateur)
);

CREATE TABLE coms_campagne (
  idcommentaire bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  idcampagne    bigint NOT NULL REFERENCES e_campagnes(idcampagne),
  libelle       varchar(200) NOT NULL,
  dhcreation    timestamptz NOT NULL DEFAULT now(),
  dhmodif       timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE l_campagnes (
  idlcampagne         bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  idcampagne          bigint        NOT NULL REFERENCES e_campagnes(idcampagne),
  num_ligne           int           NOT NULL,
  code_soc            char(3)       NOT NULL,
  numcli              numeric(12,0) NOT NULL,
  idoperateur_assigne bigint        REFERENCES operateurs(idoperateur),
  siret               varchar(14),
  raison_sociale      varchar(120),
  sous_activite       varchar(60),
  rfm                 varchar(10),
  ca_ht               numeric(14,2),
  date_dernier_achat  date,
  magasin_affilie     varchar(60),
  correspondant       varchar(80),
  telephone           varchar(25),
  email               varchar(120),
  adresse             varchar(200),
  cp                  varchar(10),
  ville               varchar(60),
  pays                varchar(60),
  langue              varchar(20),
  dhcreation          timestamptz NOT NULL DEFAULT now(),
  dhmodif             timestamptz NOT NULL DEFAULT now(),
  idoperateur_cm      bigint REFERENCES operateurs(idoperateur),
  CONSTRAINT uq_lcamp_client UNIQUE (idcampagne, code_soc, numcli),
  CONSTRAINT uq_lcamp_numligne UNIQUE (idcampagne, num_ligne)
);
CREATE INDEX ix_lcamp_campagne   ON l_campagnes(idcampagne);
CREATE INDEX ix_lcamp_client     ON l_campagnes(code_soc, numcli);
CREATE INDEX ix_lcamp_assigne    ON l_campagnes(idoperateur_assigne);
CREATE INDEX ix_lcamp_rs         ON l_campagnes(raison_sociale);
CREATE INDEX ix_lcamp_tel        ON l_campagnes(telephone);

CREATE TABLE campagnes_operateurs (
  idcampop    bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  idcampagne  bigint NOT NULL REFERENCES e_campagnes(idcampagne),
  idoperateur bigint NOT NULL REFERENCES operateurs(idoperateur),
  dhcreation  timestamptz NOT NULL DEFAULT now(),
  dhmodif     timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT uq_campop UNIQUE (idcampagne, idoperateur)
);

-- ============================================================
-- Actions
-- ============================================================
CREATE TABLE actions_campagnes (
  idactions_campagnes bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  idlcampagne         bigint      NOT NULL REFERENCES l_campagnes(idlcampagne),
  num_action          int         NOT NULL,
  dh_action           timestamptz NOT NULL,
  idoperateur         bigint      NOT NULL REFERENCES operateurs(idoperateur),
  idtype_contact      int         NOT NULL REFERENCES types_contact(idtype_contact),
  idderoulement       int         NOT NULL REFERENCES deroulements(idderoulement),
  idinteret           int         REFERENCES interets_client(idinteret),
  date_relance        date,
  date_achat          date,
  idcanal             int         REFERENCES canaux_achat(idcanal),
  idcommentaire       bigint      REFERENCES coms_campagne(idcommentaire),
  commentaire_libre   text,
  dhcreation          timestamptz NOT NULL DEFAULT now(),
  dhmodif             timestamptz NOT NULL DEFAULT now(),
  idoperateur_cm      bigint REFERENCES operateurs(idoperateur),
  CONSTRAINT uq_action_num UNIQUE (idlcampagne, num_action)
);
CREATE INDEX ix_actions_ligne   ON actions_campagnes(idlcampagne, num_action DESC);
CREATE INDEX ix_actions_relance ON actions_campagnes(date_relance)
  WHERE date_relance IS NOT NULL;

-- ============================================================
-- Verrous / opposition / journal
-- ============================================================
CREATE TABLE verrous_ligne (
  idlcampagne bigint PRIMARY KEY REFERENCES l_campagnes(idlcampagne),
  idoperateur bigint NOT NULL REFERENCES operateurs(idoperateur),
  dh_verrou   timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE clients_hors_contact (
  idclientshc    bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  soc            char(3)       NOT NULL,
  numcli         numeric(12,0) NOT NULL,
  date_exclusion date          NOT NULL DEFAULT current_date,
  CONSTRAINT uq_hors_contact UNIQUE (soc, numcli)
);

CREATE TABLE journaux_tables (
  idjournal     bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  utilisateur   varchar(50) NOT NULL,
  dhcreation    timestamptz NOT NULL DEFAULT now(),
  nom_table     varchar(60) NOT NULL,
  idtable       bigint,
  champ_table   varchar(60),
  ancienne_val  text,
  nouvelle_val  text
);
CREATE INDEX ix_journaux_cible ON journaux_tables(nom_table, idtable);

-- ============================================================
-- Vue : dernière action par ligne de campagne
-- ============================================================
CREATE VIEW v_derniere_action AS
SELECT DISTINCT ON (a.idlcampagne) a.*
FROM   actions_campagnes a
ORDER  BY a.idlcampagne, a.num_action DESC;
```

## 7.1 Jeu de données initial (seed)

```sql
INSERT INTO roles (libelle) VALUES ('Conseiller'), ('Admin');

INSERT INTO types_contact (libelle, defaut) VALUES
 ('Appel', true), ('Email', false), ('SMS', false), ('Courrier', false);

INSERT INTO deroulements (libelle) VALUES
 ('Numéro non attribué'), ('Faux numéro'), ('Entreprise fermée'),
 ('Mauvais interlocuteur'), ('Doublon'), ('Répondeur'),
 ('À rappeler'), ('Contact argumenté'), ('Ne plus contacter');

INSERT INTO interets_client (libelle) VALUES
 ('Réfractaire'), ('Intéressé via Web'), ('Intéressé via Mag'), ('Vente validée');

INSERT INTO canaux_achat (libelle) VALUES ('Web'), ('Magasin');

INSERT INTO parametres (libelle, valeur_num) VALUES
 ('POLLING_SECONDES', 5),
 ('VERROU_EXPIRATION_MINUTES', 10),
 ('ARCHIVAGE_AUTO_JOURS', 365),
 ('CONSERVATION_RGPD_JOURS', 1095);
```

**Volumétrie POC suggérée** : ~500 clients dans `v001p10` (dont quelques-uns sans téléphone pour tester les rejets d'import), 3 campagnes (1 en préparation, 1 active avec ~150 lignes partiellement traitées, 1 clôturée), 4 opérateurs (3 conseillers + 1 admin), quelques dizaines d'actions dont plusieurs rappels échus et à venir.

---

# 8. Traçabilité écrans ↔ tables

| Écran | Lecture | Écriture |
|---|---|---|
| Connexion | `OPERATEURS`, `ROLES` | — |
| Suivi des campagnes | `E_CAMPAGNES`, `L_CAMPAGNES`, `V_DERNIERE_ACTION`, `VERROUS_LIGNE`, `V001P10` (variables) | `VERROUS_LIGNE` |
| Popup de saisie d'action | `L_CAMPAGNES`, `ACTIONS_CAMPAGNES`, `TYPES_CONTACT`, `DEROULEMENTS`, `INTERETS_CLIENT`, `CANAUX_ACHAT`, `COMS_CAMPAGNE` | `ACTIONS_CAMPAGNES`, `VERROUS_LIGNE`, `CLIENTS_HORS_CONTACT`, `JOURNAUX_TABLES` |
| Rappels du jour | `V_DERNIERE_ACTION`, `L_CAMPAGNES`, `E_CAMPAGNES`, `CAMPAGNES_OPERATEURS` | — (via popup) |
| Recherche client | `L_CAMPAGNES`, `E_CAMPAGNES`, `V_DERNIERE_ACTION` (toutes campagnes) | — (via popup) |
| Tableau de bord | `E_CAMPAGNES`, `L_CAMPAGNES`, `ACTIONS_CAMPAGNES` | — |
| Admin — Gestion des campagnes | toutes tables campagne, `V001P10`, `OPERATEURS`, `CLIENTS_HORS_CONTACT` | `E_CAMPAGNES`, `L_CAMPAGNES`, `CAMPAGNES_OPERATEURS`, `JOURNAUX_TABLES` |
| Admin — Paramètres | toutes tables paramètres, `VERROUS_LIGNE`, `CLIENTS_HORS_CONTACT` | idem + `JOURNAUX_TABLES` |
| Admin — Exports & éditions | `E_CAMPAGNES`, `L_CAMPAGNES`, `ACTIONS_CAMPAGNES`, `OPERATEURS` | — |

---

# 9. Plan de construction du POC (proposition)

| Lot | Contenu | Dépendances |
|---|---|---|
| **0** | Solution .NET/Blazor, EF Core + Npgsql, DDL + seed, layout (barre supérieure + onglets), page de connexion POC | — |
| **1** | Admin — Gestion des campagnes : onglet entêtes (CRUD + statuts) | 0 |
| **2** | Admin — Import Excel/CSV avec chaîne de contrôles + chargement dans `L_CAMPAGNES` | 1 |
| **3** | Admin — Affectation opérateurs, onglet lignes, Assigner ligne | 2 |
| **4** | Composant `PopupSaisieAction` + `ServiceVerrous` + dépendances conditionnelles | 0 |
| **5** | Suivi des campagnes : 2 tables, Prochain contact, Relance, Historique, polling, verrou visible | 3, 4 |
| **6** | Rappels du jour + badge compteur permanent | 4, 5 |
| **7** | Recherche client transverse | 4 |
| **8** | Tableau de bord | 5 |
| **9** | Admin — Paramètres (listes de valeurs + hors contact + verrous actifs) | 0 |
| **10** | Admin — Exports Excel + édition PDF | 5 |
| **11** | Journalisation (intercepteur EF Core) | tous |

---

# 10. Points à arbitrer

Divergences, imprécisions ou manques relevés lors de la fusion. À valider avant développement.

## 10.1 Valeur de déroulement « Ne plus contacter » — ✅ ARBITRÉ (19/08/2026)
**Décision** : « Ne plus contacter » est ajouté comme **déroulement** à part entière dans `DEROULEMENTS`. Sa sélection déclenche, dans la même transaction que l'enregistrement de l'action, l'insertion du couple (SOC, NUMCLI) dans `CLIENTS_HORS_CONTACT`. Les champs Intérêt / Date de relance / Date d'achat / Canal d'achat sont alors forcés à vide et grisés. Une confirmation explicite est demandée à l'opérateur.
Impacts : §2.3, §3.3, §5.3, §6.2, §7, §7.1.

## 10.2 Contrôle « client hors contact » à l'import
La spec technique indique que les clients hors contact sont « écartés ou signalés » à l'import, mais ce motif **n'est pas listé** dans la colonne Validation de la popup d'import (UX). Ce document l'ajoute au rang 7 de la chaîne de contrôles, **non effaçable**. À confirmer.

## 10.3 Champ « Date d'achat » dans la popup — ✅ ARBITRÉ (19/08/2026)
**Décision** : le champ **Date d'achat** figure bien dans la popup de saisie d'action, activé uniquement si Intérêt = *Vente validée*, et alimente `ACTIONS_CAMPAGNES.DATE_ACHAT`. Son absence de la liste des champs côté UX était une omission.
Impacts : §3.3, §5.3. Reporté dans Confluence (UX v23).

## 10.4 Colonne « Date/heure de contact » vs « Date/heure de relance »
La spec technique liste la colonne **« Date/heure de contact »** puis précise « **Date/heure de relance** = date heure du moment de la prise de ligne par l'opérateur ». Il s'agit vraisemblablement d'une coquille : la donnée décrite correspond à `DH_ACTION` (date/heure de contact), la date de relance étant une date planifiée future. **Interprétation retenue** : colonne 12 = `DH_ACTION`, colonne 14 = `DATE_RELANCE`. À confirmer.

## 10.5 Bouton « Historique » — ✅ ARBITRÉ (19/08/2026)
**Décision** : l'historique est consultable **uniquement** dans le panneau latéral de la popup de saisie d'action. Le bloc dépliable sous la ligne du tableau, décrit dans la version initiale des specs techniques, est abandonné : la table principale reste en lecture seule et sans zone extensible. Colonnes du panneau : date, opérateur, déroulement, les plus récentes en premier.
Impacts : §4.4, §5.2, §5.3. Reporté dans Confluence (specs techniques v73).

## 10.6 Nom de la table des commentaires
Deux orthographes apparaissent : `COMS_CAMPAGNES` (remarque de `ACTIONS_CAMPAGNES`) et `COMS_CAMPAGNE` (définition de table, UX). **Retenu : `COMS_CAMPAGNE`.**

## 10.7 Bouton « Relance » et action vide
Le bouton Relance « crée une nouvelle ligne d'action ». Question : la ligne `ACTIONS_CAMPAGNES` est-elle créée **au clic** (donc potentiellement vide si l'opérateur annule) ou **à l'enregistrement de la popup** ? **Proposition retenue** : création à l'enregistrement uniquement, pour éviter les actions fantômes qui fausseraient les compteurs « Traités ».

## 10.8 Portée du compteur « Rappels du jour »
Cohérent entre les deux docs (tous les rappels dus des campagnes visibles, quel que soit le planificateur). À noter que cela signifie qu'un rappel peut être traité par un conseiller différent de celui qui l'a fixé — comportement voulu, à rappeler dans la formation utilisateurs.

## 10.9 Actions autres que l'appel — ✅ ARBITRÉ (19/08/2026)
**Décision** : ajout d'une table de paramètres `TYPES_CONTACT` (Appel par défaut, Email, SMS, Courrier) et d'une colonne `ACTIONS_CAMPAGNES.IDTYPE_CONTACT` **NOT NULL**. Le champ est affiché dans la popup de saisie d'action, pré-rempli avec la valeur marquée « par défaut » pour préserver la rapidité de saisie. `TYPES_CONTACT` (canal du contact) reste distinct de `CANAUX_ACHAT` (canal de l'achat en cas de vente validée).
Impacts : §2.1, §2.3, §2.5, §5.3, §6.2, §7, §7.1, §8.

*Question résiduelle* : faut-il ajouter le Type de contact comme colonne affichée dans la table principale du Suivi des campagnes (colonne 12bis, à côté de Date/heure de contact) ? Ce document ne l'a pas ajouté, la liste de colonnes étant déjà longue — mais l'information devient pertinente dès lors que plusieurs canaux coexistent.

## 10.10 Numérotation des campagnes — ✅ ARBITRÉ (19/08/2026)
**Décision** : pas de code parlant. Le « numéro de campagne » de l'édition PDF et des exports est **`IDCAMPAGNE`**, la clé numérique auto-incrémentée. Aucune colonne `CODE_CAMPAGNE` n'est créée.

Conséquences :
- Les références « 26_003 » / « 26_017 » des spécifications d'origine sont à lire comme de simples illustrations, pas comme un format à implémenter.
- L'identification d'une campagne à l'écran se fait par son **nom** (`E_CAMPAGNES.NOM`), qui doit donc rester suffisamment explicite lors de la création — c'est désormais le seul libellé parlant. Recommandation d'usage (non contraignante techniquement) : préfixer le nom par l'année et le mois, ex. « 2026-06 Promotions CHR ».
- L'`IDCAMPAGNE` est affiché dans l'en-tête du PDF à titre de référence technique (utile pour un échange support ou un rapprochement avec la BI).

## 10.11 Statut d'une ligne de campagne
Aucun statut n'est stocké sur `L_CAMPAGNES` : « traité / non traité » se déduit de l'existence d'une action. Acceptable pour le POC ; à surveiller côté performance si les campagnes dépassent quelques milliers de lignes (une colonne dénormalisée `TRAITE boolean` ou `IDDERNIERE_ACTION` serait alors utile).

## 10.12 Champ `IDOPERATEUR` vs `IDOPERATEUR_CM` dans `ACTIONS_CAMPAGNES`
Les deux colonnes coexistent. Interprétation retenue : `IDOPERATEUR` = conseiller **ayant réalisé le contact** (donnée métier, alimente la colonne « Opérateur en cours ») ; `IDOPERATEUR_CM` = auteur de la dernière **écriture technique** (peut être l'admin qui corrige). À confirmer.

## 10.13 Périmètre de « Recherche client » et RGPD — ✅ ARBITRÉ (19/08/2026)
**Décision** : la portée élargie (toutes campagnes, y compris non visibles) est **maintenue pour tous les rôles**, conseillers compris. Le cas d'usage — un client qui rappelle tombe sur le premier conseiller disponible — le justifie fonctionnellement.
Impacts : §5.5.

**Réserve à porter à la connaissance du DPO.** La journalisation est volontairement limitée aux écritures (§2.6 : « pas de traçage des lectures »). Combinée à une recherche ouverte sur l'intégralité de la base client, cela signifie qu'**aucune trace n'existe** de qui a consulté les données de quel client. Deux options, à trancher avant mise en production (le POC peut s'en dispenser) :
1. **Journaliser les recherches** dans une table dédiée (`JOURNAUX_RECHERCHES` : opérateur, critère saisi, horodatage, nombre de résultats). Coût faible, périmètre limité — on trace l'acte de recherche, pas chaque lecture d'écran. C'est l'option recommandée.
2. **Assumer l'absence de trace**, en s'appuyant sur le faible effectif (6 à 10 personnes identifiées, toutes salariées du siège) et sur la nature B2B des données. Défendable, mais fragilise la réponse à une éventuelle demande de la CNIL sur les accès.

Cette réserve ne remet pas en cause la décision fonctionnelle ci-dessus ; elle porte uniquement sur la traçabilité qui l'accompagne.

## 10.14 Durée de conservation RGPD
Non figée : « à définir avec le DPO avant la mise en production ». Le POC utilise 3 ans (paramètre `CONSERVATION_RGPD_JOURS`), sans mécanisme de purge implémenté.

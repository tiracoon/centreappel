# Spécifications à préciser

Points relevés en cours de développement où la spec (technique ou UX) ne tranche pas clairement un comportement. Une décision provisoire est prise pour pouvoir avancer, documentée ici avec son contexte — à faire arbitrer officiellement plus tard et reporter dans les specs techniques/UX une fois validé (cf. le pattern déjà utilisé en §10 de [Spécifications techniques fusionnées.md](Spécifications%20techniques%20fusionnées.md)).

---

## 2026-09-03 — Blocage du bouton Relance sur une ligne « Ne plus contacter »

**Constat** : rien dans la spec n'interdit explicitement de créer une nouvelle action (bouton Relance) sur une ligne dont la dernière action a pour déroulement « Ne plus contacter ». Le §3.4 (Exclusivité d'écriture) dit même l'inverse : un conseiller peut à tout moment créer une nouvelle action sur la ligne, sans restriction énoncée liée au déroulement précédent. Le mécanisme `CLIENTS_HORS_CONTACT` (§2.3, §3.6) est explicitement un filtre **à l'import** (empêche le client de réapparaître dans une *future* campagne), pas un blocage de la ligne déjà présente dans la campagne en cours.

**Décision provisoire retenue** : le bouton Relance est désactivé quand la ligne sélectionnée a pour dernier déroulement « Ne plus contacter », et la ligne est surlignée en rouge très pâle dans la grille (`rgba(220, 53, 69, 0.08)`). Implémenté dans `SuiviCampagnes.razor` / `.razor.cs` (`LigneNePlusContacter`, `ClasseLigne`).

**Sections concernées** : §2.3, §3.3, §3.4, §3.6, §10.2 (Spécifications techniques fusionnées).

**À trancher officiellement** : faut-il permettre de recontacter un client « Ne plus contacter » via Relance dans certains cas (ex. réservé à l'admin, cohérent avec « réversible uniquement par l'admin » au §3.3), ou bloquer totalement le bouton pour tous les rôles comme fait actuellement ?

---

## 2026-09-03 — `DATE_RELANCE` passe de DATE à DATE+HEURE

**Constat** : la spec technique (§2.3, §3.3, §5.4, requête SQL de §5.4) et l'UX décrivent systématiquement `DATE_RELANCE` comme une simple **date** (`current_date`, format `dd/MM/yyyy`), sans composante horaire. La maquette de l'écran Rappels du jour fournie par l'utilisateur affiche pourtant une heure de relance (ex. « 18/08 · 09:30 »), ce qui suppose de pouvoir planifier un rappel à un moment précis de la journée, pas juste « ce jour-là ».

**Décision retenue** : `DATE_RELANCE` devient une date-heure (`timestamp with time zone` en base, stockée en UTC comme tous les timestamps du projet), sur `ACTIONS_CAMPAGNES.DATE_RELANCE` et partout où la valeur circule (`SaisieAction`, `LigneCampagnePopup`, `LigneCampagneEnCours`, `RappelDuJour`, vue `v_derniere_action`). Conversion UTC ↔ heure locale (France, `Europe/Paris`) centralisée dans `CentreAppel.Web.Application.Extensions.DateTimeExtensions` (`HeureLocaleVersUtc`/`UtcVersHeureLocale`), appliquée à la saisie (popup, `<input type="datetime-local">`) et à l'affichage (Suivi des campagnes, Rappels du jour) — jamais stockée ni comparée en heure locale.

**Impact sur le critère de sélection de Rappels du jour** (§5.4) : la spec compare `DATE_RELANCE <= current_date` (au jour civil près). Avec une heure, la comparaison se fait maintenant contre l'instant précis (`DateTime.UtcNow`) : un rappel prévu à 16h aujourd'hui n'apparaît dans la liste/le compteur qu'à partir de 16h, pas dès 00h00 comme le ferait une comparaison au jour civil.

**Migration** : `DateRelanceEnDateHeure` (colonne `actions_campagne.date_relance` : `date` → `timestamp with time zone`). La vue `v_derniere_action` a dû être supprimée puis recréée à l'identique dans la migration (PostgreSQL refuse un `ALTER COLUMN TYPE` sur une colonne dont une vue dépend, même en `SELECT *`).

**Sections concernées** : §2.3, §3.3, §5.4 (requête SQL et colonne « Date/heure de relance » — le libellé UX employait déjà « heure », c'est la donnée qui ne suivait pas), §10.8.

**À trancher officiellement** : la spec doit être mise à jour pour refléter une vraie composante horaire sur `DATE_RELANCE`/`DATE_HEURE_RELANCE` (renommage de colonne éventuel à envisager), et le comportement du critère de sélection de Rappels du jour (jour civil vs instant précis) doit être validé explicitement.

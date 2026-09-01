CentreAppel.Web.sln
│
├── Application
│   ├── Extensions
│   │   └── ClaimsPrincipalExtensions.cs
│   ├── Models
│   │   ├── CampagneEncours.cs
│   │   ├── CanalAchat.cs
│   │   ├── CodesReferentiels.cs
│   │   ├── CommentaireCampagne.cs
│   │   ├── ConnexionFormModel.cs
│   │   ├── Deroulement.cs
│   │   ├── HistoriqueAction.cs
│   │   ├── InteretClient.cs
│   │   ├── LigneCampagneEnCours.cs
│   │   ├── LigneCampagnePopup.cs
│   │   ├── Operateur.cs
│   │   └── TypeContact.cs
│   ├── Services
│   │   ├── AuthentificationService.cs
│   │   ├── IAuthentificationService.cs
│   │   ├── CampagneService.cs
│   │   ├── ICampagneService.cs
│   │   ├── CanalAchatService.cs
│   │   ├── ClientService.cs
│   │   ├── DeroulementService.cs
│   │   ├── JournauxTablesService.cs
│   │   ├── OperateurService.cs
│   │   ├── ParametresService.cs
│   │   ├── RoleService.cs
│   │   └── VerrousLigneService.cs
│   └── services.md
│
├── Data
│   ├── Entites
│   │   ├── AuditableEntity.cs
│   │   ├── AuditableEntityWithOperateur.cs
│   │   ├── ActionCampagneEntity.cs
│   │   ├── CampagneEntity.cs
│   │   ├── CampagneOperateurEntity.cs
│   │   ├── CanalAchatEntity.cs
│   │   ├── ClientHorsContactEntity.cs
│   │   ├── CommentaireCampagneEntity.cs
│   │   ├── DerniereActionEntity.cs
│   │   ├── DeroulementEntity.cs
│   │   ├── InteretClientEntity.cs
│   │   ├── LigneCampagneEntity.cs
│   │   ├── OperateurEntity.cs
│   │   ├── ParametreEntity.cs
│   │   ├── RoleEntity.cs
│   │   ├── TypeContactEntity.cs
│   │   └── VerrouLigneEntity.cs
│   ├── Configurations
│   │   └── (une classe IEntityTypeConfiguration<T> par entité, même nom + suffixe Configuration)
│   ├── Migrations
│   │   ├── ***20260813153435_InitialCreate
│   │   ├── ***
│   │   └── ApplicationDbContextModelSnapshot.cs
│   └── Context
│       ├── ApplicationDbContext.cs
│       └── DbSeeder.cs
│
├── Enumerations
│   └── StatutCampagne.cs
│
├── Components
│   ├── App.razor
│   ├── Routes.razor
│   ├── _Imports.razor
│   ├── Layout
│   │   ├── MainLayout.razor / .razor.cs / .razor.css
│   │   ├── MenuHorizontal.razor / .razor.cs / .razor.css
│   │   └── ReconnectModal.razor / .razor.css / .razor.js
│   └── Pages
│       ├── LocalizedPage.cs
│       ├── Pages.md
│       ├── Administration
│       │   ├── Administration.razor
│       │   └── Administration.razor.cs
│       ├── Authentification
│       │   ├── Connexion.razor
│       │   └── Connexion.razor.cs
│       ├── SuiviCampagnes
│       │   ├── SuiviCampagnes.razor
│       │   ├── SuiviCampagnes.razor.cs
│       │   └── SuiviCampagnes.razor.css
│       ├── RappelsDuJour
│       │   ├── RappelsDuJour.razor
│       │   ├── RappelsDuJour.razor.cs
│       │   └── RappelsDuJour.razor.css
│       ├── RechercheClient
│       │   ├── RechercheClient.razor
│       │   └── RechercheClient.razor.cs
│       ├── TableauDeBord
│       │   ├── TableauDeBord.razor
│       │   └── TableauDeBord.razor.cs
│       ├── Shared
│       │   └── PopupSaisieAction.razor / .razor.cs / .razor.css
│       └── Commun
│           ├── AccesRefuse.razor
│           ├── Error.razor / .razor.cs
│           └── NotFound.razor / .razor.cs
│
├── Resources
│   ├── SharedResources.cs
│   └── SharedResources.resx
│
├── appsettings.json
├── appsettings.Development.json
├── Classes.md
├── Program.cs
└── CentreAppel.Web.csproj

---

## Changements notables vs plan initial

Ce fichier décrit l'arborescence telle qu'elle existe réellement (2026-08-29), après plusieurs
ajustements décidés en cours de route. Écarts principaux avec la toute première version de ce
document :

- **`Persistence/` → `Data/`** : renommé tel quel (mêmes sous-dossiers `Entites`, `Configurations`,
  `Migrations`, `Context`).
- **Suffixe `Entity`** sur toutes les classes du dossier `Entites` (`Campagne` → `CampagneEntity`,
  etc.), pour lever toute ambiguïté avec les classes de `Application/Models` qui portent souvent
  le même nom court (ex. `Deroulement` en DTO vs `DeroulementEntity` en entité). `AuditableEntity`/
  `AuditableEntityWithOperateur` ne suivent pas cette règle (déjà "Entity" dans leur nom), ni
  l'enum `StatutCampagne` (pas une entité mappée).
- **`Application/{Module}/Dto/*.cs` + `{Module}Service.cs` → `Application/Models/` +
  `Application/Services/`** : un seul dossier plat pour tous les DTOs, un seul dossier plat pour
  tous les services (au lieu d'un sous-dossier par module métier). Certains services prévus
  séparément ont été fusionnés au fil de l'eau dans `CampagneService` (ex. `ActionsCampagnesService`
  supprimé, sa méthode `GetHistoriqueAsync` déplacée dans `CampagneService`).
- **`StatutCampagne` déplacé dans `Enumerations/`** (namespace `CentreAppel.Web.Enum`), en dehors
  de `Data/Entites`, puisque ce n'est pas une entité mappée mais un type de valeur partagé.
  `StatutCampagne` était à l'origine un `varchar`, converti en `int`-backed enum par migration.
- **Colonne `Code` remplace `Libelle`** sur les référentiels fermés (`Role`, `TypeContact`,
  `Deroulement`, `InteretClient`, `CanalAchat`) : identité métier stable (ex. `CONTACT_ARGUMENTE`),
  utilisée comme clé de traduction `.resx` — l'affichage ne dépend plus d'un texte stocké en base.
  `CommentaireCampagne.Libelle` et `Parametre.Libelle` restent en l'état : texte libre par
  campagne pour l'un, identifiant technique déjà stable pour l'autre — aucun des deux n'est un
  référentiel fermé au sens de cette règle.
- **`CampagneEnCours`/`LigneCampagneEnCours`** (recherche affichage) sont en réalité dans
  `Application/Models/CampagneEncours.cs` / `LigneCampagneEnCours.cs` — noms de fichiers legacy,
  pas strictement alignés sur `Classes.md`, non renommés depuis.
- Le dossier `Components/Pages/Shared` ne contient plus que `PopupSaisieAction` (l'ancien
  `AccesRefuse.razor` dupliqué dans 3 dossiers du plan initial a été tranché : une seule instance,
  dans `Commun/`).

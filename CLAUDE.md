# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Stack

- .NET 10, C# 14
- EF Core 10 (`Microsoft.EntityFrameworkCore.Design`, `Npgsql.EntityFrameworkCore.PostgreSQL`)
- PostgreSQL 18 (port 5432, locale fr-FR)
- Ne pas utiliser .NET Aspire

## Commandes courantes

```
dotnet build
dotnet run
dotnet watch run
dotnet ef migrations add <NomMigration>
dotnet ef database update
dotnet user-secrets set "ConnectionStrings:Default" "<connection-string>"
```

## Architecture logicielle:
Monolithe modulaire
Un seul projet déployable, une seule base.
Utilisation de services injectés
Suivre l'architecture présente dans le fichier Architectureprojet.md

## Règles structurelles:
-Les fichiers .razor ne contiennent que du visuel. Le bloc `@code` du fichier Razor reste vide.
-Les fichiers .razor.cs appelle le service concerné et gere l'état d'affichage.
-Les services appelent le dbContext.
-Application des principes SOLID
-Pas d'utilisation de pattern complexe du genre CQRS,Mediatr ou autre
- Async de bout en bout : `async`/`await`, suffixe `Async`, `CancellationToken` sur
  les méthodes de service.
- Nullable reference types activé, et respecté — pas de `!` pour faire taire le
  compilateur.
- Pas de commentaire qui paraphrase le code. Un commentaire explique un *pourquoi*
  non évident, notamment une règle métier surprenante.
- Injection de dépendances par constructeur.


## Comportement
- Compiler après chaque modification.
- Exécuter les tests après chaque fonctionnalité.
- Ne modifier que le périmètre demandé.
- Ne rien modifier silencieusement. Si quelque chose a été modifié dans le code c'est que c'est le developpeur qui l'a fait.
- Quand plusieurs approches sont possibles et que le choix a des conséquences
  durables, signaler l'alternative plutôt que trancher silencieusement (Signaler toute ambiguïté avant d'implémenter)

## Conventions de nommage

| Élément | Langue | Exemple |
|---|---|---|
| Types techniques, méthodes, propriétés | anglais | `AppelService`, `GetByIdAsync`, `IsActive` |
| Concepts métier | français | `Appel`, `Campagne`, `Prospect`, `Relance` |
| Textes affichés à l'utilisateur | français, via `.resx` | jamais de littéral dans un composant |
| Tables et colonnes PostgreSQL | minuscules + underscore | `appel`, `date_appel`, `campagne_id` |

Signature typique : `AppelService.GetByCampagneAsync(int campagneId)`.
Ne pas produire d'hybrides comme `ObtenirParId` ou `AppelDTO`.

### Glossaire verbe métier → anglais
Le verbe est toujours en anglais — y compris dans un gestionnaire d'événement UI (`On...`, lié à
`@onclick`, `@bind:after`). Seul le nom métier (`Rappel`, `Campagne`, `Opérateur`, `Verrou`, `Ligne`,
`Action`, `ProchainContact`, `Relance`, `Historique`...) reste en français.
Ne jamais traduire un terme technique C#/.NET (`Get`, `On`, `Handle`, `Async`, `Task`...).

Traductions à utiliser systématiquement :

| Français | Anglais | Exemple |
|---|---|---|
| Obtenir / Récupérer | Get | `GetRappelsDuJourAsync` |
| Compter | Count | `CountRappelsDuJourAsync` |
| Créer | Create | `CreateCampagneAsync` |
| Modifier / Mettre à jour | Update | `UpdateOperateurAsync` |
| Supprimer | Delete | `DeleteLigneCampagneAsync` |
| Charger | Load | `LoadHistoriqueAsync` |
| Enregistrer / Valider | Save / Submit | `SaveActionAsync` |
| Rechercher | Search / Find | `SearchClientAsync` |
| Vérifier / Contrôler | Check / Validate | `ValidateActionAsync` |
| Acquérir | Acquire | `AcquireVerrouAsync` |
| Libérer | Release | `ReleaseVerrouAsync` |
| Traiter | Handle | `OnHandleAsync` |
| Envoyer | Send | `SendNotificationAsync` |

Exemple concret : le clic sur le bouton « Traiter » (Rappels du jour) se nomme `OnHandleAsync`, jamais `OnTraiterAsync`. En revanche `OnProchainContactAsync` reste correct : « Prochain contact » est un nom métier (le libellé du bouton dans la spec), pas un verbe.

Si un concept a plusieurs formulations possibles (ex. « Rappels du jour » vs « Rappels dus »), choisir une seule forme et l'utiliser partout — classe de service, interface, méthodes, DTOs — sans variation.

## Conventions base de données

- `UseSnakeCaseNamingConvention` pour le mapping des tables/colonnes.
- Nom des tables dans la base en minuscule. Exemple:  Nom entité C#: DateAppel Nom postgres: date_appel le SQL écrit à la main reste lisible et n'exige pas de guillemets
- Stocker tous les timestamps en UTC.
- Le mot de passe de connexion à la base ne doit jamais apparaître dans `appsettings.json` : le stocker via .NET user secrets (`dotnet user-secrets`).
- Toute évolution de schéma passe par les migrations EF Core (`dotnet ef migrations add` / `dotnet ef database update`).

## Affichage
Conversion en heure locale à l'affichage uniquement.

## Charte graphique
Suivre les spécifications UX Design spécifiées dans le document Documents\Spécification UX.pdf

Suivre les consignes présentes dans les fichiers .md aux emplacements ci-dessous:
CentreAppel.Web\ArchitectureProjet.md
CentreAppel.Web\Classes.md
CentreAppel.Web\Services.md
CentreAppel.Web\Components\Pages\Pages.md






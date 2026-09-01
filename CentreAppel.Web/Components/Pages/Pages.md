### Ce fichier sert de guide pour la construction des Pages razors et razor.cs

### fichier .razor 
Une page .razor doit contenir uniquement ce qui concerne la présentation et pas de code.
Toute page qui contient du texte doit etre localisée. Y compris les données de statut présent dans les select.
Si un doute subsiste quant à la traduction, poser la question avant d'écrire le code.

### .razor.cs
Important! Une page .razor.cs ne doit pas appeler le DbContext.
Ne jamais exposer une entité EF Core à l'UI.

Une page .razor.cs doit appeler le ou les services correspondants à ce qui va être affichés. 
Exemple:
    [Inject]
    private ICampagneService CampagneService { get; set; } = default!;

Si le service n'existe pas demander à le créer et si le developpeur est d'accord le créer selon les directives contenues dans le fichier Application/Services.md

Si la classe qui doit contenir les données n'existe pas, la créer dans le dossier Dto selon les directives contenues dans le fichier Classes.md

### Localisation
Si une page contient du texte, sa classe .razor.cs doit hériter de `LocalizedPage`
(dossier `Components/Pages/LocalizedPage.cs`), qui injecte déjà `Localizer` :
public partial class Connexion : LocalizedPage

Le fichier .razor correspondant doit alors déclarer `@inherits LocalizedPage`, sinon
le partiel généré par le .razor hérite de ComponentBase par défaut et le compilateur
refuse les deux déclarations partielles (bases différentes).
Exemple:
	@inherits LocalizedPage

Puis pour chaque texte à localiser ajouter la commande @Localizer
Exemple:
	@Localizer["SuiviCampagnes_ColonneCampagne"]





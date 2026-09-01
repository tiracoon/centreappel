namespace CentreAppel.Web.Data.Entites;

public class InteretClientEntity : AuditableEntity
{
    public int IdInteret { get; set; }

    // Identité métier stable (ex. "VENTE_VALIDEE") — clé de traduction .resx, remplace
    // Libelle. C'est sur ce champ que la popup de saisie d'action teste l'Intérêt,
    // jamais sur l'Id.
    public required string Code { get; set; }
}

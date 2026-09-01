namespace CentreAppel.Web.Data.Entites;

public class DeroulementEntity : AuditableEntity
{
    public int IdDeroulement { get; set; }

    // Identité métier stable (ex. "CONTACT_ARGUMENTE") — clé de traduction .resx, remplace
    // Libelle. C'est sur ce champ que la popup de saisie d'action teste le Déroulement,
    // jamais sur l'Id.
    public required string Code { get; set; }
}

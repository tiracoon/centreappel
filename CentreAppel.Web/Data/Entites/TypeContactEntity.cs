namespace CentreAppel.Web.Data.Entites;

public class TypeContactEntity : AuditableEntity
{
    public int IdTypeContact { get; set; }
    public bool Defaut { get; set; }

    // Identité métier stable (ex. "APPEL") — clé de traduction .resx, remplace Libelle.
    public required string Code { get; set; }
}

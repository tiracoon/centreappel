namespace CentreAppel.Web.Data.Entites;

public class CanalAchatEntity : AuditableEntity
{
    public int IdCanal { get; set; }

    // Identité métier stable (ex. "WEB") — clé de traduction .resx, remplace Libelle.
    public required string Code { get; set; }
}

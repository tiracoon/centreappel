namespace CentreAppel.Web.Data.Entites;

public class RoleEntity : AuditableEntity
{
    public int IdRole { get; set; }

    // Identité métier stable (ex. "ADMINISTRATEUR") — clé de traduction .resx, testée par
    // [Authorize(Roles = "...")] et par le claim ClaimTypes.Role à la connexion.
    public required string Code { get; set; }
}

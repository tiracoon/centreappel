namespace CentreAppel.Web.Data.Entites;

public class OperateurEntity : AuditableEntity
{
    public long IdOperateur { get; set; }
    public required string LoginAd { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public int IdRole { get; set; }
    public RoleEntity Role { get; set; } = null!;
}

namespace CentreAppel.Web.Data.Entites;

public abstract class AuditableEntity
{
    public DateTime DhCreation { get; set; }
    public DateTime? DhModif { get; set; }
}

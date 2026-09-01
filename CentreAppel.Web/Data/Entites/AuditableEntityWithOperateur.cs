namespace CentreAppel.Web.Data.Entites;

public abstract class AuditableEntityWithOperateur : AuditableEntity
{
    public long IdOperateurCm { get; set; }
}

using CentreAppel.Web.Enum;

namespace CentreAppel.Web.Data.Entites;

public class CampagneEntity : AuditableEntityWithOperateur
{
    public long IdCampagne { get; set; }
    public required string Nom { get; set; }
    public DateOnly DateCampagne { get; set; }
    public required string Description { get; set; }
    public StatutCampagne Statut { get; set; }
}

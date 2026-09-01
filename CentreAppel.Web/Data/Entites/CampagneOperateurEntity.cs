namespace CentreAppel.Web.Data.Entites;

// Visibilité d'une campagne par opérateur — à ne pas confondre avec
// LigneCampagne.IdOperateurAssigne, qui réserve une ligne précise à traiter.
public class CampagneOperateurEntity : AuditableEntity
{
    public long IdCampagneOperateur { get; set; }
    public long IdCampagne { get; set; }
    public CampagneEntity Campagne { get; set; } = null!;
    public long IdOperateur { get; set; }
    public OperateurEntity Operateur { get; set; } = null!;
}

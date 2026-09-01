namespace CentreAppel.Web.Data.Entites;

// Une ligne ne peut porter qu'un verrou à la fois : IdLCampagne fait office de PK naturelle.
public class VerrouLigneEntity
{
    public long IdLCampagne { get; set; }
    public LigneCampagneEntity LigneCampagne { get; set; } = null!;
    public long IdOperateur { get; set; }
    public OperateurEntity Operateur { get; set; } = null!;
    public DateTime DhVerrou { get; set; }
}

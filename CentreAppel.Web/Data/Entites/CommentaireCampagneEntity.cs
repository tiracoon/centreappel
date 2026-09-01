namespace CentreAppel.Web.Data.Entites;

public class CommentaireCampagneEntity : AuditableEntity
{
    public long IdCommentaire { get; set; }
    public long IdCampagne { get; set; }
    public CampagneEntity Campagne { get; set; } = null!;
    public required string Libelle { get; set; }
}

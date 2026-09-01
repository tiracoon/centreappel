namespace CentreAppel.Web.Data.Entites;

// Modèle de lecture seule mappé sur la vue PostgreSQL v_derniere_action
// (dernière action par ligne de campagne, cf. migration).
public class DerniereActionEntity
{
    public long IdActionsCampagnes { get; set; }
    public long IdLCampagne { get; set; }
    public int NumAction { get; set; }
    public DateTime DhAction { get; set; }
    public long IdOperateur { get; set; }
    public int IdTypeContact { get; set; }
    public int IdDeroulement { get; set; }
    public int? IdInteret { get; set; }
    public DateOnly? DateRelance { get; set; }
    public DateOnly? DateAchat { get; set; }
    public int? IdCanal { get; set; }
    public long? IdCommentaire { get; set; }
    public string? CommentaireLibre { get; set; }
    public DateTime DhCreation { get; set; }
    public DateTime? DhModif { get; set; }
    public long IdOperateurCm { get; set; }
}

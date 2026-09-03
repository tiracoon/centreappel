namespace CentreAppel.Web.Data.Entites;

public class ActionCampagneEntity : AuditableEntityWithOperateur
{
    public long IdActionsCampagnes { get; set; }
    public long IdLCampagne { get; set; }
    public LigneCampagneEntity LigneCampagne { get; set; } = null!;
    public int NumAction { get; set; }
    public DateTime DhAction { get; set; }

    // Opérateur ayant réalisé le contact (donnée métier, alimente « Opérateur en cours »).
    // Distinct de IdOperateurCm (hérité), qui trace l'auteur de la dernière écriture technique.
    public long IdOperateur { get; set; }
    public OperateurEntity Operateur { get; set; } = null!;

    public int IdTypeContact { get; set; }
    public TypeContactEntity TypeContact { get; set; } = null!;

    public int IdDeroulement { get; set; }
    public DeroulementEntity Deroulement { get; set; } = null!;

    // Conditionné à Déroulement = « Contact argumenté »
    public int? IdInteret { get; set; }
    public InteretClientEntity? Interet { get; set; }

    // Conditionné à Déroulement = « À rappeler » — date ET heure (stockée en UTC, cf.
    // CentreAppel.Web.Application.Extensions.DateTimeExtensions pour la conversion à
    // l'affichage/la saisie).
    public DateTime? DateRelance { get; set; }

    // Conditionné à Intérêt = « Vente validée »
    public DateOnly? DateAchat { get; set; }
    public int? IdCanal { get; set; }
    public CanalAchatEntity? Canal { get; set; }

    public long? IdCommentaire { get; set; }
    public CommentaireCampagneEntity? Commentaire { get; set; }
    public string? CommentaireLibre { get; set; }
}

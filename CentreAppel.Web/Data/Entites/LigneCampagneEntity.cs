namespace CentreAppel.Web.Data.Entites;

public class LigneCampagneEntity : AuditableEntityWithOperateur
{
    public long IdLCampagne { get; set; }
    public long IdCampagne { get; set; }
    public CampagneEntity Campagne { get; set; } = null!;
    public int NumLigne { get; set; }

    // Import
    public required string CodeSoc { get; set; }
    public decimal NumCli { get; set; }

    // Admin — NULL = ligne libre
    public long? IdOperateurAssigne { get; set; }
    public OperateurEntity? OperateurAssigne { get; set; }

    // AS/400 — figé à l'import
    public string? Siret { get; set; }
    public string? RaisonSociale { get; set; }
    public string? SousActivite { get; set; }
    public string? MagasinAffilie { get; set; }
    public string? Correspondant { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public string? Cp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? Langue { get; set; }

    // AS/400 — variable, relu à l'ouverture
    public string? Rfm { get; set; }
    public decimal? CaHt { get; set; }
    public DateOnly? DateDernierAchat { get; set; }
}

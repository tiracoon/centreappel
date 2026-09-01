namespace CentreAppel.Web.Data.Entites;

public class ParametreEntity : AuditableEntity
{
    public int IdParametre { get; set; }
    public required string Libelle { get; set; }
    public string? ValeurTexte { get; set; }
    public decimal? ValeurNum { get; set; }
}

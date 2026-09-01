namespace CentreAppel.Web.Application.Models;

public class HistoriqueAction
{
    public DateTime DhAction { get; set; }
    public required string LoginOperateur { get; set; }
    public required string DeroulementCode { get; set; }
}

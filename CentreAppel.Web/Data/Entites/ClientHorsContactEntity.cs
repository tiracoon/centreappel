namespace CentreAppel.Web.Data.Entites;

public class ClientHorsContactEntity
{
    public long IdClientsHc { get; set; }
    public required string Soc { get; set; }
    public decimal NumCli { get; set; }
    public DateOnly DateExclusion { get; set; }
}

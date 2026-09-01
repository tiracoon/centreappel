namespace CentreAppel.Web.Application.Models;

public class TypeContact
{
    public int IdTypeContact { get; set; }
    public required string Code { get; set; }
    public bool Defaut { get; set; }
}

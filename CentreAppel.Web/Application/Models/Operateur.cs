namespace CentreAppel.Web.Application.Models
{
    public class Operateur
    {
        public long OperateurId { get; set; }
        public required string LoginAd { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}

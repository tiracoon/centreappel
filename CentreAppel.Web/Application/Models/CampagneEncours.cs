using CentreAppel.Web.Enum;

namespace CentreAppel.Web.Application.Models
{
    public class CampagneEnCours
    {
        public long IdCampagne { get; set; }
        public required string Nom { get; set; }
        public DateOnly DateCampagne { get; set; }
        public required string Description { get; set; }
        public int NbLignes { get; set; }
        public int NbATraiter { get; set; }
        public StatutCampagne Statut { get; set; }
    }

}

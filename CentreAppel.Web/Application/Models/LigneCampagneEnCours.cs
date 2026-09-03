namespace CentreAppel.Web.Application.Models
{
    public class LigneCampagneEnCours
    {
        public long IdLCampagne { get; set; }
        public long IdCampagne { get; set; }
        public string? CodeSoc { get; set; }
        public decimal? NumCli { get; set; }
        public string? Rfm { get; set; }
        public string? RaisonSociale { get; set; }
        public string? SousActivite { get; set; }
        public decimal? CaHt { get; set; }
        public DateOnly? DateDernierAchat { get; set; }
        public string? Correspondant { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? MagasinAffilie { get; set; }

        public DateOnly? DateHeureContact { get; set; }
        
        //public int IdTypeContact { get; set; }
        public string? TypeContactCode { get; set; }

        //public int IdDeroulement { get; set; }
        public string? DeroulementCode { get; set; }

        public DateTime? DateRelance { get; set; }
        public string? InteretClientCode { get; set; }
        public string? CanalAchatCode { get; set; }
        public string? Commentaire { get; set; }

        //public long? IdOperateur { get; set; }
        public string? NomOperateurEnCours { get; set; }
    }
}
namespace CentreAppel.Web.Application.Models;

public class LigneCampagnePopup
{
    public long IdLCampagne { get; set; }
    public long IdCampagne { get; set; }
    public required string RaisonSociale { get; set; }
    public required string CodeSoc { get; set; }
    public decimal NumCli { get; set; }
    public string? Telephone { get; set; }
    public string? Rfm { get; set; }
    public string? MagasinAffilie { get; set; }
    public string? Correspondant { get; set; }

    // Dernière action existante sur cette ligne — sert à pré-remplir SaisieAction en modification.
    // Tout à null si la ligne n'a encore aucune action (première saisie).
    public int? IdTypeContact { get; set; }
    public int? IdDeroulement { get; set; }
    public int? IdInteret { get; set; }
    public DateOnly? DateRelance { get; set; }
    public DateOnly? DateAchat { get; set; }
    public int? IdCanal { get; set; }
    public long? IdCommentaire { get; set; }
}

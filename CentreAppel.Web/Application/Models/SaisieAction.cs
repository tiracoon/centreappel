namespace CentreAppel.Web.Application.Models;

// Regroupe tout ce que CampagneService.SaveActionAsync enregistre pour une tentative de contact.
// Créé/rempli par PopupSaisieAction : à partir de la dernière action existante si la ligne a déjà
// été traitée (modification), vide sinon (création).
public class SaisieAction
{
    public long IdLCampagne { get; set; }
    public long IdOperateur { get; set; }
    public int? IdTypeContact { get; set; }
    public int? IdDeroulement { get; set; }
    public int? IdInteret { get; set; }
    public DateOnly? DateRelance { get; set; }
    public DateOnly? DateAchat { get; set; }
    public int? IdCanal { get; set; }
    public long? IdCommentaire { get; set; }
}

using CentreAppel.Web.Enum;

namespace CentreAppel.Web.Application.Models;

// Ligne affichée dans /recherche-client (§5.5) : une par campagne (active ou clôturée) où le
// client figure, y compris celles non visibles par l'opérateur ailleurs dans l'application
// (§10.13 : portée élargie, tous rôles). Mêmes colonnes que Rappels du jour, mais le déroulement
// n'est pas fixé par construction ici (pas seulement "À rappeler") : il faut donc le porter.
public class ResultatRechercheClient
{
    public long IdLCampagne { get; set; }
    public string NomCampagne { get; set; } = string.Empty;
    public StatutCampagne StatutCampagne { get; set; }
    public string? RaisonSociale { get; set; }
    public string? Telephone { get; set; }

    // Renseignée uniquement si le client a un rappel en cours (dernière action de déroulement
    // "À rappeler") ; vide sinon.
    public DateTime? DateRelance { get; set; }

    public string? DeroulementDerniereActionCode { get; set; }
    public string? LoginOperateurDerniereAction { get; set; }
}

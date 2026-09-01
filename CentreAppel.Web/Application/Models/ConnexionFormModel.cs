namespace CentreAppel.Web.Application.Models;


public sealed class ConnexionFormModel
{
    public string LoginAd { get; set; } = string.Empty;

    // Saisi mais non contrôlé : POC en attente de l'intégration AD.
    public string MotDePasse { get; set; } = string.Empty;
}

using CentreAppel.Web.Application.Extensions;
using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Application.Services;
using CentreAppel.Web.Enum;
using Microsoft.AspNetCore.Components;

namespace CentreAppel.Web.Components.Pages.RechercheClient
{
    public partial class RechercheClient : LocalizedPage
    {
        [Inject]
        private RechercheClientService RechercheClientService { get; set; } = default!;

        private string TermeRecherche { get; set; } = string.Empty;
        private bool RechercheLancee { get; set; }
        private List<ResultatRechercheClient> Resultats { get; set; } = [];
        private long? IdLCampagnePopupOuverte { get; set; }

        // Seul point d'entrée de cette page vers la popup : toujours en mode Traiter (§5.5), comme
        // Rappels du jour.
        private ModeOuverturePopup ModePopup { get; } = ModeOuverturePopup.Traiter;

        private async Task OnSearchAsync()
        {
            RechercheLancee = true;
            Resultats = await RechercheClientService.SearchClientAsync(TermeRecherche, CancellationToken.None);
        }

        private static bool CampagneActive(ResultatRechercheClient resultat) => resultat.StatutCampagne == StatutCampagne.Active;

        private void OnHandle(ResultatRechercheClient resultat)
        {
            IdLCampagnePopupOuverte = resultat.IdLCampagne;
        }

        private async Task OnPopupClosedAsync()
        {
            IdLCampagnePopupOuverte = null;
            await OnSearchAsync();
        }

        private async Task OnPopupSavedAsync()
        {
            IdLCampagnePopupOuverte = null;
            await OnSearchAsync();
        }

        // DateRelance est stockée en UTC ; conversion à l'affichage uniquement (cf. DateTimeExtensions).
        private string DateRelanceAffichee(ResultatRechercheClient resultat) =>
            resultat.DateRelance is null ? string.Empty : resultat.DateRelance.Value.UtcVersHeureLocale().ToString("dd/MM · HH:mm");

        private static bool EstEnRetard(ResultatRechercheClient resultat) => resultat.DateRelance is not null && resultat.DateRelance < DateTime.UtcNow;
    }
}

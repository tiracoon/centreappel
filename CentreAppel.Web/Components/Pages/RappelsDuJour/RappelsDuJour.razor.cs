using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Application.Services;
using CentreAppel.Web.Enum;
using Microsoft.AspNetCore.Components;

namespace CentreAppel.Web.Components.Pages.RappelsDuJour
{
    public partial class RappelsDuJour : LocalizedPage
    {
        [Inject]
        private RappelsDuJourService RappelsDuJourService { get; set; } = default!;

        [Inject]
        private ILogger<RappelsDuJour> Logger { get; set; } = default!;

        private List<RappelDuJour> Rappels { get; set; } = [];
        private long? IdLCampagnePopupOuverte { get; set; }

        // Seul point d'entrée de cette page vers la popup : toujours en mode Traiter (§5.4).
        private ModeOuverturePopup ModePopup { get; } = ModeOuverturePopup.Traiter;

        protected override async Task OnParametersSetAsync() => await LoadAsync();

        private async Task LoadAsync()
        {
            var idOperateur = await GetIdOperateurConnecteAsync();
            if (idOperateur is null) return;

            Rappels = await RappelsDuJourService.GetRappelsDuJourAsync(idOperateur.Value, CancellationToken.None);
        }

        private async Task OnHandleAsync(RappelDuJour rappel)
        {
            Logger.LogInformation("Opérateur {IdOperateur} clique sur Traiter pour la ligne {IdLCampagne}", await GetIdOperateurConnecteAsync(), rappel.IdLCampagne);

            IdLCampagnePopupOuverte = rappel.IdLCampagne;
        }

        private async Task OnPopupClosedAsync()
        {
            IdLCampagnePopupOuverte = null;
            await LoadAsync();
        }

        private async Task OnPopupSavedAsync()
        {
            IdLCampagnePopupOuverte = null;
            await LoadAsync();
        }

        private bool EstEnRetard(RappelDuJour rappel) => rappel.DateRelance < DateOnly.FromDateTime(DateTime.Now);
    }
}

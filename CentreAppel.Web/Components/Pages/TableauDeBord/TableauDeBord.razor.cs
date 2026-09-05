using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Application.Services;
using Microsoft.AspNetCore.Components;

namespace CentreAppel.Web.Components.Pages.TableauDeBord
{
    public partial class TableauDeBord : LocalizedPage
    {
        [Inject]
        private ICampagneService CampagneService { get; set; } = default!;

        private List<CampagneEnCours> CampagnesDisponibles { get; set; } = [];
        private long? IdCampagneSelectionnee { get; set; }
        private TableauDeBordDonnees Donnees { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            var idOperateur = await GetIdOperateurConnecteAsync();
            if (idOperateur is null) return;

            CampagnesDisponibles = await CampagneService.GetCampagnesAsync(idOperateur.Value, CancellationToken.None);

            // Sélectionne la première campagne (les plus récentes en tête, cf. GetCampagnesAsync)
            // si aucune sélection explicite n'a encore été faite.
            IdCampagneSelectionnee ??= CampagnesDisponibles.FirstOrDefault()?.IdCampagne;

            await ChargerStatistiquesAsync();
        }

        private async Task OnCampagneChange(ChangeEventArgs e)
        {
            IdCampagneSelectionnee = long.TryParse(e.Value?.ToString(), out var id) ? id : null;
            await ChargerStatistiquesAsync();
        }

        private async Task ChargerStatistiquesAsync()
        {
            Donnees = IdCampagneSelectionnee is null
                ? new TableauDeBordDonnees()
                : await CampagneService.GetStatistiquesCampagneAsync(IdCampagneSelectionnee.Value, CancellationToken.None);
        }
    }
}

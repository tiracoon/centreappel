using CentreAppel.Web.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CentreAppel.Web.Components.Layout;

public partial class MenuHorizontal : IDisposable
{
    // Résolu par MainLayout via AuthorizeView (AuthenticationState), pas via [CascadingParameter]
    // HttpContext — ce dernier devient null dès qu'un composant InteractiveServer reçoit une
    // interaction sur le circuit SignalR (cf. LocalizedPage).
    [Parameter]
    public long? IdOperateur { get; set; }

    [Inject]
    private IStringLocalizer<SharedResources> Localizer { get; set; } = default!;

    [Inject]
    private RappelsDuJourService RappelsDuJourService { get; set; } = default!;

    private static readonly TimeSpan IntervalleRafraichissement = TimeSpan.FromSeconds(5);

    private int NombreRappelsDuJour { get; set; }

    private PeriodicTimer? timerRafraichissement;

    protected override async Task OnParametersSetAsync() => await ChargerCompteurAsync();

    protected override void OnInitialized()
    {
        // Le badge doit rester à jour même quand l'opérateur reste sur une autre page (§5.4 :
        // "toujours visible... rafraîchi par polling") — timer propre au menu, indépendant du
        // polling de la page actuellement affichée.
        timerRafraichissement = new PeriodicTimer(IntervalleRafraichissement);
        _ = RafraichirPeriodiquementAsync();
    }

    private async Task RafraichirPeriodiquementAsync()
    {
        while (await timerRafraichissement!.WaitForNextTickAsync())
        {
            await InvokeAsync(async () =>
            {
                await ChargerCompteurAsync();
                StateHasChanged();
            });
        }
    }

    private async Task ChargerCompteurAsync()
    {
        if (IdOperateur is null) return;

        NombreRappelsDuJour = await RappelsDuJourService.CountRappelsDuJourAsync(IdOperateur.Value, CancellationToken.None);
    }

    public void Dispose() => timerRafraichissement?.Dispose();
}

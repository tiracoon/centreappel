using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CentreAppel.Web.Components.Layout;

public partial class MenuHorizontal
{
    [Inject]
    private IStringLocalizer<SharedResources> Localizer { get; set; } = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private int NombreRappelsDuJour { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //var idOperateur = HttpContext.User.GetIdOperateurConnecte();
        //if (idOperateur is not null)
        //{
        //    NombreRappelsDuJour = await RappelsDuJourService.CountRappelsDuJourAsync(idOperateur.Value, CancellationToken.None);
        //}
    }
}

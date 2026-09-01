using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CentreAppel.Web.Components.Layout;

public partial class MainLayout
{
    private object Model { get; } = new();

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedResources> Localizer { get; set; } = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private async Task OnDeconnexionAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        NavigationManager.NavigateTo("/connexion", forceLoad: true);
    }
}

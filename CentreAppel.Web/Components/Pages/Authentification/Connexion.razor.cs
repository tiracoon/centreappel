using System.Security.Claims;
using CentreAppel.Web.Application.Services;
using CentreAppel.Web.Application.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;


namespace CentreAppel.Web.Components.Pages.Authentification;

public partial class Connexion : LocalizedPage
{
    [SupplyParameterFromForm]
    private ConnexionFormModel Model { get; set; } = default!;

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [Inject]
    private IAuthentificationService AuthentificationService { get; set; } = default!;

    protected override void OnInitialized() => Model ??= new();

    private string? ErreurConnexion;

    private async Task OnValidSubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(Model.LoginAd))
        {
            ErreurConnexion = Localizer["Connexion_LoginRequis"];
            return;
        }

        var operateur = await AuthentificationService.AuthenticateAsync(Model.LoginAd, HttpContext.RequestAborted);
        if (operateur is null)
        {
            ErreurConnexion = Localizer["Connexion_LoginInconnu"];
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, operateur.IdOperateur.ToString()),
            new(ClaimTypes.Name, $"{operateur.FirstName} {operateur.LastName}"),
            new("login_ad", operateur.LoginAd),
            new(ClaimTypes.Role, operateur.Role.Code),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        NavigationManager.NavigateTo(ReturnUrl ?? "/", forceLoad: true);
    }
}

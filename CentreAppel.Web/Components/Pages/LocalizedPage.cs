using CentreAppel.Web.Application.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace CentreAppel.Web.Components.Pages;

public abstract class LocalizedPage : ComponentBase
{
    [Inject]
    protected IStringLocalizer<SharedResources> Localizer { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    // Uniquement valable pendant le rendu SSR statique initial (ex. Connexion.razor, pour
    // HttpContext.SignInAsync). Devient null dès qu'un composant @rendermode InteractiveServer
    // reçoit une interaction sur le circuit SignalR — utiliser GetIdOperateurConnecteAsync
    // (AuthenticationStateProvider) pour identifier l'utilisateur dans ce cas.
    [CascadingParameter]
    protected HttpContext HttpContext { get; set; } = default!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    // Fonctionne dans les deux modes de rendu (SSR statique et InteractiveServer), contrairement
    // à HttpContext.User.
    protected async Task<long?> GetIdOperateurConnecteAsync()
    {
        var authState = await AuthenticationStateTask;
        return authState.User.GetIdOperateurConnecte();
    }
}

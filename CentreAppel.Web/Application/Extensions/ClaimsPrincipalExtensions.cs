using System.Security.Claims;

namespace CentreAppel.Web.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long? GetIdOperateurConnecte(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return claim is not null && long.TryParse(claim.Value, out var id) ? id : null;
    }
}

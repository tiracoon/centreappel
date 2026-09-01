using CentreAppel.Web.Data.Entites;

namespace CentreAppel.Web.Application.Services;

public interface IAuthentificationService
{
    Task<OperateurEntity?> AuthenticateAsync(string loginAd, CancellationToken cancellationToken);
}

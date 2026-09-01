using CentreAppel.Web.Data.Context;
using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class AuthentificationService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IAuthentificationService
{
    public async Task<OperateurEntity?> AuthenticateAsync(string loginAd, CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        // POC : seul le login AD est vérifié, le mot de passe n'est pas encore contrôlé.
        return await context.Operateurs
            .Include(o => o.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.LoginAd == loginAd && o.IsActive, cancellationToken);
    }
}

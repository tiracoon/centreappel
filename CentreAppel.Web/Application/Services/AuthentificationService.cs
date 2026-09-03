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
        // Comparaison insensible à la casse : ToLower() des deux côtés se traduit en LOWER(...)
        // côté SQL, donc reste indépendant de la collation de la colonne (contrairement à ILIKE,
        // spécifique PostgreSQL).
        var loginAdMinuscule = loginAd.ToLower();
        return await context.Operateurs
            .Include(o => o.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.LoginAd.ToLower() == loginAdMinuscule && o.IsActive, cancellationToken);
    }
}

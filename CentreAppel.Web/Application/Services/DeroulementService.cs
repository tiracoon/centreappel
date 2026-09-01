using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class DeroulementService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<Deroulement>> GetDeroulementsAsync(CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Deroulements
            .AsNoTracking()
            .OrderBy(d => d.IdDeroulement)
            .Select(d => new Deroulement
            {
                IdDeroulement = d.IdDeroulement,
                Code = d.Code
            })
            .ToListAsync(cancellationToken);
    }
}

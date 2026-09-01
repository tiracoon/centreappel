using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class CanalAchatService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<CanalAchat>> GetCanauxAchatAsync(CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.CanauxAchat
            .AsNoTracking()
            .OrderBy(c => c.IdCanal)
            .Select(c => new CanalAchat
            {
                IdCanal = c.IdCanal,
                Code = c.Code
            })
            .ToListAsync(cancellationToken);
    }
}

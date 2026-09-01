using CentreAppel.Web.Application.Models;
using CentreAppel.Web.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Application.Services;

public class ClientService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<InteretClient>> GetInteretsClientAsync(CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.InteretsClient
            .AsNoTracking()
            .OrderBy(i => i.IdInteret)
            .Select(i => new InteretClient
            {
                IdInteret = i.IdInteret,
                Code = i.Code
            })
            .ToListAsync(cancellationToken);
    }
    public async Task<List<TypeContact>> GetTypesContactAsync(CancellationToken cancellationToken)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.TypesContact
            .AsNoTracking()
            .OrderBy(t => t.IdTypeContact)
            .Select(t => new TypeContact
            {
                IdTypeContact = t.IdTypeContact,
                Code = t.Code,
                Defaut = t.Defaut
            })
            .ToListAsync(cancellationToken);
    }
}

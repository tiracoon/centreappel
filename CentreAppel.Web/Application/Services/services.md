### Ce fichier sert de guide pour la construction des Services

Seul le service peut avoir accès au DBContext par injection par constructeur 

Exemple:
public class CampagneService(IDbContextFactory<ApplicationDbContext> dbContextFactory) 
{
	await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
}

Le service doit remonter le plus possible des informations pertinentes et le minimum vital pour ne pas surcharger les échanges.

Par exemple pour afficher une campagne:

public async Task<CampagneEnCours?> GetCampagnesEnCoursAsync(long idOperateur, StatutCampagne statutCampagne , CancellationToken cancellationToken)
{
	await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
	return await context.Campagnes
            .AsNoTracking()
            .Where(c => c.Statut == statutCampagne)
            .Where(c => context.CampagnesOperateur.Any(co => co.IdCampagne == c.IdCampagne && co.IdOperateur == idOperateur))
            .OrderBy(c => c.DateCampagne)
            .ToListAsync();
}

Un service doit posséder une interface puis etre injecté en DI dans Program.cs

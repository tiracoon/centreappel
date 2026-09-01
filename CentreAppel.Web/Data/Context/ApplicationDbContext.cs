using CentreAppel.Web.Application.Extensions;
using CentreAppel.Web.Data.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CentreAppel.Web.Data.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    public DbSet<OperateurEntity> Operateurs => Set<OperateurEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<CampagneEntity> Campagnes => Set<CampagneEntity>();
    public DbSet<LigneCampagneEntity> LignesCampagne => Set<LigneCampagneEntity>();
    public DbSet<DeroulementEntity> Deroulements => Set<DeroulementEntity>();
    public DbSet<TypeContactEntity> TypesContact => Set<TypeContactEntity>();
    public DbSet<InteretClientEntity> InteretsClient => Set<InteretClientEntity>();
    public DbSet<CanalAchatEntity> CanauxAchat => Set<CanalAchatEntity>();
    public DbSet<CommentaireCampagneEntity> CommentairesCampagne => Set<CommentaireCampagneEntity>();
    public DbSet<CampagneOperateurEntity> CampagnesOperateur => Set<CampagneOperateurEntity>();
    public DbSet<ActionCampagneEntity> ActionsCampagne => Set<ActionCampagneEntity>();
    public DbSet<DerniereActionEntity> DernieresActions => Set<DerniereActionEntity>();
    public DbSet<VerrouLigneEntity> VerrousLigne => Set<VerrouLigneEntity>();
    public DbSet<ClientHorsContactEntity> ClientsHorsContact => Set<ClientHorsContactEntity>();
    public DbSet<ParametreEntity> Parametres => Set<ParametreEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var maintenant = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DhCreation = maintenant;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.DhModif = maintenant;
            }
        }

        var idOperateurConnecte = httpContextAccessor.HttpContext?.User.GetIdOperateurConnecte();
        if (idOperateurConnecte is not null)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntityWithOperateur>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.IdOperateurCm = idOperateurConnecte.Value;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

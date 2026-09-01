using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasKey(r => r.IdRole);
        builder.Property(r => r.Code).HasMaxLength(30).IsRequired();
        builder.HasIndex(r => r.Code).IsUnique();
    }
}

using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class CanalAchatConfiguration : IEntityTypeConfiguration<CanalAchatEntity>
{
    public void Configure(EntityTypeBuilder<CanalAchatEntity> builder)
    {
        builder.HasKey(c => c.IdCanal);
        builder.Property(c => c.Code).HasMaxLength(40).IsRequired();
        builder.HasIndex(c => c.Code).IsUnique();
    }
}

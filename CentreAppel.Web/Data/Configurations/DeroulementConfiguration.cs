using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class DeroulementConfiguration : IEntityTypeConfiguration<DeroulementEntity>
{
    public void Configure(EntityTypeBuilder<DeroulementEntity> builder)
    {
        builder.HasKey(d => d.IdDeroulement);
        builder.Property(d => d.Code).HasMaxLength(40).IsRequired();
        builder.HasIndex(d => d.Code).IsUnique();
    }
}

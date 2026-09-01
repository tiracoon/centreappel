using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class InteretClientConfiguration : IEntityTypeConfiguration<InteretClientEntity>
{
    public void Configure(EntityTypeBuilder<InteretClientEntity> builder)
    {
        builder.HasKey(i => i.IdInteret);
        builder.Property(i => i.Code).HasMaxLength(40).IsRequired();
        builder.HasIndex(i => i.Code).IsUnique();
    }
}

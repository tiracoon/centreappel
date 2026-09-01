using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class ClientHorsContactConfiguration : IEntityTypeConfiguration<ClientHorsContactEntity>
{
    public void Configure(EntityTypeBuilder<ClientHorsContactEntity> builder)
    {
        builder.HasKey(c => c.IdClientsHc);
        builder.Property(c => c.Soc).HasColumnType("character(3)").IsRequired();
        builder.Property(c => c.NumCli).HasPrecision(12, 0);

        builder.HasIndex(c => new { c.Soc, c.NumCli }).IsUnique();
    }
}

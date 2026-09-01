using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class DerniereActionConfiguration : IEntityTypeConfiguration<DerniereActionEntity>
{
    public void Configure(EntityTypeBuilder<DerniereActionEntity> builder)
    {
        builder.HasNoKey();
        builder.ToView("v_derniere_action");
    }
}

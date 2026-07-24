using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations;

public class MstBrandConfiguration : BaseConfiguration<MstBrand>
{
    public override void Configure(EntityTypeBuilder<MstBrand> builder)
    {
        base.Configure(builder);

        builder.ToTable("mst_brands", "dev");

        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();
    }
}

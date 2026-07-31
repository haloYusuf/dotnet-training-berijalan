using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class DealerConfiguration : BaseConfiguration<MstDealer>
    {
        public override void Configure(EntityTypeBuilder<MstDealer> builder)
        {
            base.Configure(builder);

            builder.ToTable("mst_dealer", "dev");

            builder.Property(c => c.Code).HasColumnName("code");
            builder.Property(c => c.Name).HasColumnName("name");
            builder.Property(c => c.Address).HasColumnName("address");
            builder.Property(c => c.City).HasColumnName("city");
            builder.Property(c => c.Region).HasColumnName("region");
            builder.Property(c => c.Phone).HasColumnName("phone");
            builder.Property(c => c.Email).HasColumnName("email");
        }
    }
}
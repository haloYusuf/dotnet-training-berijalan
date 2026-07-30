using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class TypeConfiguration : BaseConfiguration<MstType>
    {
        public override void Configure(EntityTypeBuilder<MstType> builder)
        {
            base.Configure(builder);

            // 2. Definisi nama tabel dan schema
            builder.ToTable("mst_types", "dev");

            // 3. Mapping kolom secara eksplisit ke huruf kecil (snake_case)
            builder.Property(t => t.BrandId).HasColumnName("brand_id");
            builder.Property(t => t.Code).HasColumnName("code");
            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.Year).HasColumnName("year");

            // 4. Setup Relasi Foreign Key ke tabel Brand
            builder.HasOne(t => t.Brand)
                   .WithMany()
                   .HasForeignKey(t => t.BrandId);
        }
    }
}
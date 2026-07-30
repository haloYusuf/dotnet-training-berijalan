using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class ModelConfiguration : BaseConfiguration<MstModel>
    {
        public override void Configure(EntityTypeBuilder<MstModel> builder)
        {
            base.Configure(builder);

            builder.ToTable("mst_models", "dev");

            // Mapping eksplisit ke huruf kecil (snake_case)
            builder.Property(m => m.TypeId).HasColumnName("type_id");
            builder.Property(m => m.Code).HasColumnName("code");
            builder.Property(m => m.Name).HasColumnName("name");
            builder.Property(m => m.Year).HasColumnName("year");
            builder.Property(m => m.Price).HasColumnName("price");
            builder.Property(m => m.Stock).HasColumnName("stock");

            // Relasi Foreign Key ke tabel Type
            builder.HasOne(m => m.Type)
                   .WithMany()
                   .HasForeignKey(m => m.TypeId);
        }
    }
}
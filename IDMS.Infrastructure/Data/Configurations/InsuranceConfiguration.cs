using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class InsuranceConfiguration : BaseConfiguration<MstInsurance>
    {
        public override void Configure(EntityTypeBuilder<MstInsurance> builder)
        {
            base.Configure(builder);

            builder.ToTable("mst_insurance", "dev");

            // 3. Mapping kolom spesifik ke snake_case
            builder.Property(c => c.Code).HasColumnName("code").IsRequired();
            builder.Property(c => c.Name).HasColumnName("name").IsRequired();
            builder.Property(c => c.CoverageType).HasColumnName("coverage_type").IsRequired();
            builder.Property(c => c.Rate).HasColumnName("rate").IsRequired();

        }
    }
}
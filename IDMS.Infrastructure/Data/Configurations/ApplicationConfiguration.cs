using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class ApplicationConfiguration : BaseConfiguration<TrnApplication>
    {
        public override void Configure(EntityTypeBuilder<TrnApplication> builder)
        {
            base.Configure(builder);

            builder.ToTable("trn_applications", "dev");

            builder.Property(c => c.ApplicationNo).HasColumnName("application_no").IsRequired();

            builder.Property(c => c.CustomerId).HasColumnName("customer_id").IsRequired();

            builder.Property(c => c.ModelId).HasColumnName("model_id").IsRequired();

            builder.Property(c => c.OtrPrice).HasColumnName("otr_price").IsRequired();

            builder.Property(c => c.DpAmount).HasColumnName("dp_amount").IsRequired();

            builder.Property(c => c.TenorMonth).HasColumnName("tenor_month").IsRequired();

            builder.Property(c => c.InterestRate).HasColumnName("interest_rate").IsRequired();

            builder.Property(c => c.Status).HasColumnName("status").HasMaxLength(20).IsRequired();

            builder.HasOne(t => t.Customer)
                   .WithMany()
                   .HasForeignKey(t => t.CustomerId);

            builder.HasOne(t => t.Model)
                   .WithMany()
                   .HasForeignKey(t => t.ModelId);
        }

    }
}
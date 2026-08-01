using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class VehicleDeliveryConfiguration : BaseConfiguration<TrnVehicleDelivery>
    {
        public override void Configure(EntityTypeBuilder<TrnVehicleDelivery> builder)
        {
            base.Configure(builder);

            builder.ToTable("trn_vehicle_delivery", "dev");

            builder.Property(c => c.DeliveryNo).HasColumnName("delivery_no").IsRequired();

            builder.Property(c => c.ApplicationId).HasColumnName("application_id").IsRequired();

            builder.Property(c => c.DealerId).HasColumnName("dealer_id").IsRequired();

            builder.Property(c => c.InsuranceId).HasColumnName("insurance_id").IsRequired();

            builder.Property(c => c.DeliveryDate).HasColumnName("delivery_date").IsRequired();

            builder.Property(c => c.DriverName).HasColumnName("driver_name").IsRequired();

            builder.Property(c => c.DriverPhone).HasColumnName("driver_phone").HasMaxLength(15);

            builder.Property(c => c.PlatNumber).HasColumnName("plat_number").HasMaxLength(15);

            builder.Property(c => c.Status).HasColumnName("status").HasMaxLength(20).IsRequired();

            builder.Property(c => c.Notes).HasColumnName("notes").HasMaxLength(255);

            builder.HasOne(t => t.Application)
                   .WithMany()
                   .HasForeignKey(t => t.ApplicationId);

            builder.HasOne(t => t.Dealer)
                   .WithMany()
                   .HasForeignKey(t => t.DealerId);

            builder.HasOne(t => t.Insurance)
                   .WithMany()
                   .HasForeignKey(t => t.InsuranceId);
        }
    }
}
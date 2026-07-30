using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : BaseConfiguration<MstCustomer>
    {
        public override void Configure(EntityTypeBuilder<MstCustomer> builder)
        {
            // 1. Panggil BaseConfiguration untuk mapping DeletedAt, CreatedAt, dll
            base.Configure(builder);

            // 2. Mapping ke nama tabel sesuai SQL-mu (tanpa 's')
            builder.ToTable("mst_customer", "dev");

            // 3. Mapping kolom spesifik ke snake_case
            builder.Property(c => c.Nik).HasColumnName("nik");
            builder.Property(c => c.FullName).HasColumnName("full_name");
            builder.Property(c => c.BirthDate).HasColumnName("birth_date");
            builder.Property(c => c.Phone).HasColumnName("phone");
            builder.Property(c => c.Email).HasColumnName("email");
            builder.Property(c => c.Address).HasColumnName("address");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDMS.Infrastructure.Data.Configurations
{
    public class UserConfiguration : BaseConfiguration<MstUser>
    {
        public override void Configure(EntityTypeBuilder<MstUser> builder)
        {
            base.Configure(builder);

            builder.ToTable("mst_users", "dev");

            // Mapping eksplisit ke huruf kecil (snake_case)
            builder.Property(u => u.Email).HasColumnName("email");
            builder.Property(u => u.Password).HasColumnName("password");
            builder.Property(u => u.FullName).HasColumnName("full_name");
            builder.Property(u => u.FullName).HasColumnName("full_name");
            builder.Property(u => u.Role).HasMaxLength(10).HasColumnName("role");
        }
    }
}
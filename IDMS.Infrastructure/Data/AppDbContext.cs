using IDMS.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IDMS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MstBrand> MstBrands => Set<MstBrand>();

    public DbSet<MstType> MstTypes => Set<MstType>();

    public DbSet<MstModel> MstModels => Set<MstModel>();

    public DbSet<MstUser> MstUsers => Set<MstUser>();

    public DbSet<MstCustomer> MstCustomers => Set<MstCustomer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

using Microsoft.EntityFrameworkCore;
using Kiosk.Data.Configurations;
using KioskEntities = Kiosk.Entities;

namespace Kiosk.Data;

public class KioskDbContext : DbContext
{
    public KioskDbContext(DbContextOptions<KioskDbContext> options) : base(options) { }

    public DbSet<KioskEntities.Customer> Customers { get; set; }
    public DbSet<KioskEntities.Kiosk> Kiosks { get; set; }
    public DbSet<KioskEntities.Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KioskDbContext).Assembly);
    }

}
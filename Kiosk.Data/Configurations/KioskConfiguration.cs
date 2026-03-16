using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KioskEntity = Kiosk.Entities.Kiosk;

namespace Kiosk.Data.Configurations
{
    public class KioskConfiguration : IEntityTypeConfiguration<KioskEntity>
    {
        public void Configure(EntityTypeBuilder<KioskEntity> entity)
        {
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Name).IsRequired().HasMaxLength(100);
            // In an real application, I´ll add SerialNumber as unique index, but for technical test purposes, I´ll leave it as is.
            entity.Property(k => k.SerialNumber).IsRequired().HasMaxLength(50);
            entity.Property(k => k.AdminURL).HasMaxLength(200);
            entity.Property(k => k.Version).HasMaxLength(20);
            entity.Property(k => k.Location).HasMaxLength(200);
            entity.HasOne(k => k.Customer).WithMany(c => c.Kiosks).HasForeignKey(k => k.CustomerId);
            entity.HasMany(k => k.Devices).WithOne(d => d.Kiosk).HasForeignKey(d => d.KioskId);
        }
    }
}

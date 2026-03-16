using Kiosk.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kiosk.Data.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> entity)
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FirmwareVersion).HasMaxLength(20);
            entity.Property(d => d.Brand).IsRequired().HasMaxLength(50);
            entity.Property(d => d.Model).IsRequired().HasMaxLength(50);
            entity.Property(d => d.DeviceType).IsRequired().HasMaxLength(50);
            entity.HasOne(d => d.Kiosk).WithMany(k => k.Devices).HasForeignKey(d => d.KioskId);
        }
    }
}

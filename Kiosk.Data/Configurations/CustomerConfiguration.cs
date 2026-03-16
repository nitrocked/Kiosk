using Kiosk.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kiosk.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.HasKey(c => c.Id);
            // In an real application, I´ll add Name as unique index, but for technical test purposes, I´ll leave it as is.
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.CIF).IsRequired().HasMaxLength(20);
            entity.HasMany(c => c.Kiosks).WithOne(k => k.Customer).HasForeignKey(k => k.CustomerId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Kiosk.Entities;
using KioskEntity = Kiosk.Entities.Kiosk;

namespace Kiosk.Data;

public static class KioskDbContextSeed
{
    public static async Task SeedAsync(KioskDbContext context, bool enabled = true)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        if (!enabled) return;

        // Seed Customers
        var customerSeeds = new[]
        {
            new Customer { Name = "Demo Customer A", CIF = "CIF-000001" },
            new Customer { Name = "Demo Customer B", CIF = "CIF-000002" }
        };

        var customerMap = new Dictionary<string, Customer>(StringComparer.OrdinalIgnoreCase);

        foreach (var seed in customerSeeds)
        {
            var existing = await context.Customers.FirstOrDefaultAsync(c => c.Name == seed.Name);
            if (existing == null)
            {
                context.Customers.Add(seed);
                customerMap[seed.Name] = seed;
            }
            else
            {
                customerMap[seed.Name] = existing;
            }
        }

        // Seed Kiosks
        var kioskSeeds = new[]
        {
            new
            {
                Name = "Demo Kiosk A1",
                SerialNumber = "KIOSK-A1",
                AdminURL = "https://localhost:5001",
                AdminPort = 5001,
                Version = "1.0.0",
                Location = "Lobby",
                CustomerName = "Demo Customer A"
            },
            new
            {
                Name = "Demo Kiosk A2",
                SerialNumber = "KIOSK-A2",
                AdminURL = "https://localhost:5002",
                AdminPort = 5002,
                Version = "1.0.0",
                Location = "Hallway",
                CustomerName = "Demo Customer A"
            },
            new
            {
                Name = "Demo Kiosk B1",
                SerialNumber = "KIOSK-B1",
                AdminURL = "https://localhost:5003",
                AdminPort = 5003,
                Version = "1.0.0",
                Location = "Reception",
                CustomerName = "Demo Customer B"
            }
        };

        var kioskMap = new Dictionary<string, KioskEntity>(StringComparer.OrdinalIgnoreCase);
        foreach (var seed in kioskSeeds)
        {
            var existing = await context.Kiosks.FirstOrDefaultAsync(k => k.Name == seed.Name);
            if (existing == null)
            {
                var customer = customerMap[seed.CustomerName];
                var kiosk = new KioskEntity
                {
                    Name = seed.Name,
                    SerialNumber = seed.SerialNumber,
                    AdminURL = seed.AdminURL,
                    AdminPort = seed.AdminPort,
                    Version = seed.Version,
                    Location = seed.Location,
                    Customer = customer
                };

                context.Kiosks.Add(kiosk);
                kioskMap[seed.Name] = kiosk;
            }
            else
            {
                kioskMap[seed.Name] = existing;
            }
        }

        // Seed Devices
        var deviceSeeds = new[]
        {
            new
            {
                Name = "Demo Device A1-1",
                SerialNumber = "DEVICE-A1-1",
                DeviceType = "POS",
                Brand = "Acme",
                Model = "X1000",
                FirmwareVersion = "1.0.0",
                KioskName = "Demo Kiosk A1"
            },
            new
            {
                Name = "Demo Device A2-1",
                SerialNumber = "DEVICE-A2-1",
                DeviceType = "POS",
                Brand = "Acme",
                Model = "X1001",
                FirmwareVersion = "1.0.0",
                KioskName = "Demo Kiosk A2"
            },
            new
            {
                Name = "Demo Device A2-2",
                SerialNumber = "DEVICE-A2-2",
                DeviceType = "POS",
                Brand = "Acme",
                Model = "X1002",
                FirmwareVersion = "1.0.0",
                KioskName = "Demo Kiosk A2"
            },
            new
            {
                Name = "Demo Device B1-1",
                SerialNumber = "DEVICE-B1-1",
                DeviceType = "POS",
                Brand = "Acme",
                Model = "X2000",
                FirmwareVersion = "1.0.0",
                KioskName = "Demo Kiosk B1"
            },
            new
            {
                Name = "Demo Device B1-2",
                SerialNumber = "DEVICE-B1-2",
                DeviceType = "POS",
                Brand = "Acme",
                Model = "X2000",
                FirmwareVersion = "1.0.0",
                KioskName = "Demo Kiosk B1"
            }
        };

        foreach (var seed in deviceSeeds)
        {
            var existing = await context.Devices.FirstOrDefaultAsync(d => d.Name == seed.Name);
            if (existing == null)
            {
                var kiosk = kioskMap[seed.KioskName];
                var device = new Device
                {
                    Name = seed.Name,
                    SerialNumber = seed.SerialNumber,
                    DeviceType = seed.DeviceType,
                    Brand = seed.Brand,
                    Model = seed.Model,
                    FirmwareVersion = seed.FirmwareVersion,
                    Kiosk = kiosk
                };

                context.Devices.Add(device);
            }
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }
}

using Kiosk.Domain.DTOs;
using Kiosk.Domain.Services;

namespace Kiosk.Domain.Tests.Services;

public class KioskServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistKiosk()
    {
        using var context = TestHelpers.CreateContext(nameof(CreateAsync_ShouldPersistKiosk));
        var service = new KioskService(context, TestHelpers.CreateMapper());

        var dto = new CreateKioskDto
        {
            Name = "Kiosk 1",
            SerialNumber = "SN-001",
            AdminURL = "https://localhost",
            AdminPort = 1234
        };

        var created = await service.CreateAsync(dto);

        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.SerialNumber, created.SerialNumber);
        Assert.Equal(dto.AdminURL, created.AdminURL);
        Assert.Equal(dto.AdminPort, created.AdminPort);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingKiosk()
    {
        using var context = TestHelpers.CreateContext(nameof(UpdateAsync_ShouldUpdateExistingKiosk));
        var service = new KioskService(context, TestHelpers.CreateMapper());

        var createDto = new CreateKioskDto
        {
            Name = "Kiosk 1",
            SerialNumber = "SN-001",
            AdminURL = "https://localhost",
            AdminPort = 1234
        };

        var created = await service.CreateAsync(createDto);

        var updateDto = new UpdateKioskDto
        {
            Name = "Kiosk Updated",
            SerialNumber = "SN-002",
            AdminURL = "https://localhost:5001",
            AdminPort = 4321
        };

        var updated = await service.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal(updateDto.Name, updated.Name);
        Assert.Equal(updateDto.SerialNumber, updated.SerialNumber);
        Assert.Equal(updateDto.AdminURL, updated.AdminURL);
        Assert.Equal(updateDto.AdminPort, updated.AdminPort);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveKiosk()
    {
        using var context = TestHelpers.CreateContext(nameof(DeleteAsync_ShouldRemoveKiosk));
        var service = new KioskService(context, TestHelpers.CreateMapper());

        var created = await service.CreateAsync(new CreateKioskDto
        {
            Name = "To Delete",
            SerialNumber = "SN-001",
            AdminURL = "https://localhost",
            AdminPort = 1234
        });

        var deleted = await service.DeleteAsync(created.Id);
        Assert.True(deleted);

        var loaded = await service.GetByIdAsync(created.Id);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task AssignAndUnassignDeviceAsync_ShouldUpdateDeviceKioskId()
    {
        using var context = TestHelpers.CreateContext(nameof(AssignAndUnassignDeviceAsync_ShouldUpdateDeviceKioskId));
        var service = new KioskService(context, TestHelpers.CreateMapper());

        var kiosk = await service.CreateAsync(new CreateKioskDto
        {
            Name = "Kiosk 1",
            SerialNumber = "SN-001",
            AdminURL = "https://localhost",
            AdminPort = 1234
        });

        var device = new Kiosk.Entities.Device
        {
            Name = "Device 1",
            DeviceType = "Type1",
            Brand = "Brand",
            Model = "Model"
        };

        context.Devices.Add(device);
        await context.SaveChangesAsync();

        var assigned = await service.AssignDeviceAsync(kiosk.Id, device.Id);
        Assert.True(assigned);

        var deviceFromDb = await context.Devices.FindAsync(device.Id);
        Assert.NotNull(deviceFromDb);
        Assert.Equal(kiosk.Id, deviceFromDb!.KioskId);

        var unassigned = await service.UnassignDeviceAsync(kiosk.Id, device.Id);
        Assert.True(unassigned);

        deviceFromDb = await context.Devices.FindAsync(device.Id);
        Assert.NotNull(deviceFromDb);
        Assert.Null(deviceFromDb!.KioskId);
    }
}

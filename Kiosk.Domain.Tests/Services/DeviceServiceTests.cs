using Kiosk.Domain.DTOs;
using Kiosk.Domain.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kiosk.Domain.Tests.Services;

public class DeviceServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistDevice()
    {
        using var context = TestHelpers.CreateContext(nameof(CreateAsync_ShouldPersistDevice));
        var service = new DeviceService(context, TestHelpers.CreateMapper(), NullLogger<DeviceService>.Instance);

        var dto = new CreateDeviceDto
        {
            Name = "Device 1",
            SerialNumber = "D-001",
            DeviceType = "Type",
            Brand = "Brand",
            Model = "Model",
            FirmwareVersion = "1.0"
        };

        var created = await service.CreateAsync(dto);

        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.SerialNumber, created.SerialNumber);
        Assert.Equal(dto.DeviceType, created.DeviceType);
        Assert.Equal(dto.Brand, created.Brand);
        Assert.Equal(dto.Model, created.Model);
        Assert.Equal(dto.FirmwareVersion, created.FirmwareVersion);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingDevice()
    {
        using var context = TestHelpers.CreateContext(nameof(UpdateAsync_ShouldUpdateExistingDevice));
        var service = new DeviceService(context, TestHelpers.CreateMapper(), NullLogger<DeviceService>.Instance);

        var createDto = new CreateDeviceDto
        {
            Name = "Device 1",
            SerialNumber = "D-001",
            DeviceType = "Type",
            Brand = "Brand",
            Model = "Model",
            FirmwareVersion = "1.0"
        };

        var created = await service.CreateAsync(createDto);

        var updateDto = new UpdateDeviceDto
        {
            Name = "Updated",
            SerialNumber = "D-002",
            DeviceType = "NewType",
            Brand = "NewBrand",
            Model = "NewModel",
            FirmwareVersion = "2.0"
        };

        var updated = await service.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal(updateDto.Name, updated.Name);
        Assert.Equal(updateDto.SerialNumber, updated.SerialNumber);
        Assert.Equal(updateDto.DeviceType, updated.DeviceType);
        Assert.Equal(updateDto.Brand, updated.Brand);
        Assert.Equal(updateDto.Model, updated.Model);
        Assert.Equal(updateDto.FirmwareVersion, updated.FirmwareVersion);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveDevice()
    {
        using var context = TestHelpers.CreateContext(nameof(DeleteAsync_ShouldRemoveDevice));
        var service = new DeviceService(context, TestHelpers.CreateMapper(), NullLogger<DeviceService>.Instance);

        var created = await service.CreateAsync(new CreateDeviceDto
        {
            Name = "To Delete",
            SerialNumber = "D-001",
            DeviceType = "Type",
            Brand = "Brand",
            Model = "Model"
        });

        var deleted = await service.DeleteAsync(created.Id);
        Assert.True(deleted);

        var loaded = await service.GetByIdAsync(created.Id);
        Assert.Null(loaded);
    }
}

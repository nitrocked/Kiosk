using FluentAssertions;
using Kiosk.Api.Controllers;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kiosk.Api.Tests.Controllers;

public class DeviceControllerTests
{
    private readonly Mock<IDeviceService> _deviceServiceMock;
    private readonly DeviceController _controller;

    public DeviceControllerTests()
    {
        _deviceServiceMock = new Mock<IDeviceService>();
        _controller = new DeviceController(_deviceServiceMock.Object, NullLogger<DeviceController>.Instance);
    }

    [Fact]
    public async Task GetDevices_ShouldReturnOkWithDevices()
    {
        // Arrange
        var devices = new List<DeviceDto>
        {
            new() { Id = 1, Name = "Device 1", DeviceType = "Type1", Brand = "Brand1", Model = "Model1" },
            new() { Id = 2, Name = "Device 2", DeviceType = "Type2", Brand = "Brand2", Model = "Model2" }
        };
        _deviceServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(devices);

        // Act
        var result = await _controller.GetDevices();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(devices);
    }

    [Fact]
    public async Task GetDevice_ShouldReturnDevice_WhenExists()
    {
        // Arrange
        var device = new DeviceDto { Id = 1, Name = "Device 1", DeviceType = "Type1", Brand = "Brand1", Model = "Model1" };
        _deviceServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(device);

        // Act
        var result = await _controller.GetDevice(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(device);
    }

    [Fact]
    public async Task GetDevice_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _deviceServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((DeviceDto?)null);

        // Act
        var result = await _controller.GetDevice(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostDevice_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateDeviceDto { Name = "New Device", DeviceType = "Type", Brand = "Brand", Model = "Model" };
        var createdDevice = new DeviceDto { Id = 1, Name = "New Device", DeviceType = "Type", Brand = "Brand", Model = "Model" };
        _deviceServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdDevice);

        // Act
        var result = await _controller.PostDevice(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(DeviceController.GetDevice));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.Value.Should().Be(createdDevice);
    }

    [Fact]
    public async Task PutDevice_ShouldReturnOk_WhenUpdateSuccessful()
    {
        // Arrange
        var updateDto = new UpdateDeviceDto { Name = "Updated", DeviceType = "NewType", Brand = "NewBrand", Model = "NewModel" };
        var updatedDevice = new DeviceDto { Id = 1, Name = "Updated", DeviceType = "NewType", Brand = "NewBrand", Model = "NewModel" };
        _deviceServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updatedDevice);

        // Act
        var result = await _controller.PutDevice(1, updateDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task PutDevice_ShouldReturnNotFound_WhenDeviceNotFound()
    {
        // Arrange
        var updateDto = new UpdateDeviceDto { Name = "Updated", DeviceType = "NewType", Brand = "NewBrand", Model = "NewModel" };
        _deviceServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync((DeviceDto?)null);

        // Act
        var result = await _controller.PutDevice(1, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteDevice_ShouldReturnOk_WhenDeletionSuccessful()
    {
        // Arrange
        _deviceServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteDevice(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DeleteDevice_ShouldReturnNotFound_WhenDeviceNotFound()
    {
        // Arrange
        _deviceServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteDevice(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}

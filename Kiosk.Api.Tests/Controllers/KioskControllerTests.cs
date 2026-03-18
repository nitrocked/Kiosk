using FluentAssertions;
using Kiosk.Api.Controllers;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kiosk.Api.Tests.Controllers;

public class KioskControllerTests
{
    private readonly Mock<IKioskService> _kioskServiceMock;
    private readonly KioskController _controller;

    public KioskControllerTests()
    {
        _kioskServiceMock = new Mock<IKioskService>();
        _controller = new KioskController(_kioskServiceMock.Object, NullLogger<KioskController>.Instance);
    }

    [Fact]
    public async Task GetKiosks_ShouldReturnOkWithKiosks()
    {
        // Arrange
        var kiosks = new List<KioskDto>
        {
            new() { Id = 1, Name = "Kiosk 1", SerialNumber = "SN-001" },
            new() { Id = 2, Name = "Kiosk 2", SerialNumber = "SN-002" }
        };
        _kioskServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(kiosks);

        // Act
        var result = await _controller.GetKiosks();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(kiosks);
    }

    [Fact]
    public async Task GetKiosk_ShouldReturnKiosk_WhenExists()
    {
        // Arrange
        var kiosk = new KioskDto { Id = 1, Name = "Kiosk 1", SerialNumber = "SN-001" };
        _kioskServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(kiosk);

        // Act
        var result = await _controller.GetKiosk(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(kiosk);
    }

    [Fact]
    public async Task GetKiosk_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((KioskDto?)null);

        // Act
        var result = await _controller.GetKiosk(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostKiosk_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateKioskDto { Name = "New Kiosk", SerialNumber = "SN-001", AdminURL = "https://localhost", AdminPort = 1234 };
        var createdKiosk = new KioskDto { Id = 1, Name = "New Kiosk", SerialNumber = "SN-001" };
        _kioskServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdKiosk);

        // Act
        var result = await _controller.PostKiosk(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(KioskController.GetKiosk));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.Value.Should().Be(createdKiosk);
    }

    [Fact]
    public async Task PutKiosk_ShouldReturnOk_WhenUpdateSuccessful()
    {
        // Arrange
        var updateDto = new UpdateKioskDto { Name = "Updated", SerialNumber = "SN-002" };
        var updatedKiosk = new KioskDto { Id = 1, Name = "Updated", SerialNumber = "SN-002" };
        _kioskServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updatedKiosk);

        // Act
        var result = await _controller.PutKiosk(1, updateDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task PutKiosk_ShouldReturnNotFound_WhenKioskNotFound()
    {
        // Arrange
        var updateDto = new UpdateKioskDto { Name = "Updated", SerialNumber = "SN-002" };
        _kioskServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync((KioskDto?)null);

        // Act
        var result = await _controller.PutKiosk(1, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteKiosk_ShouldReturnOk_WhenDeletionSuccessful()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteKiosk(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DeleteKiosk_ShouldReturnNotFound_WhenKioskNotFound()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteKiosk(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AssignDeviceToKiosk_ShouldReturnOk_WhenAssignmentSuccessful()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.AssignDeviceAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _controller.AssignDeviceToKiosk(1, 2);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task AssignDeviceToKiosk_ShouldReturnNotFound_WhenAssignmentFails()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.AssignDeviceAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _controller.AssignDeviceToKiosk(1, 2);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be("Kiosk or Device not found.");
    }

    [Fact]
    public async Task UnassignDeviceFromKiosk_ShouldReturnOk_WhenUnassignmentSuccessful()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.UnassignDeviceAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _controller.UnassignDeviceFromKiosk(1, 2);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task UnassignDeviceFromKiosk_ShouldReturnNotFound_WhenUnassignmentFails()
    {
        // Arrange
        _kioskServiceMock.Setup(s => s.UnassignDeviceAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _controller.UnassignDeviceFromKiosk(1, 2);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be("Kiosk or Device not found, or device is not assigned to this kiosk.");
    }
}

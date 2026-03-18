using FluentAssertions;
using Kiosk.Api.Controllers;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kiosk.Api.Tests.Controllers;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _customerServiceMock = new Mock<ICustomerService>();
        _controller = new CustomerController(_customerServiceMock.Object, NullLogger<CustomerController>.Instance);
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnOkWithCustomers()
    {
        // Arrange
        var customers = new List<CustomerDto>
        {
            new() { Id = 1, Name = "Customer 1", CIF = "A12345678" },
            new() { Id = 2, Name = "Customer 2", CIF = "B87654321" }
        };
        _customerServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

        // Act
        var result = await _controller.GetCustomers();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(customers);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnCustomer_WhenExists()
    {
        // Arrange
        var customer = new CustomerDto { Id = 1, Name = "Customer 1", CIF = "A12345678" };
        _customerServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(customer);

        // Act
        var result = await _controller.GetCustomer(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(customer);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((CustomerDto?)null);

        // Act
        var result = await _controller.GetCustomer(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostCustomer_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateCustomerDto { Name = "New Customer", CIF = "A12345678" };
        var createdCustomer = new CustomerDto { Id = 1, Name = "New Customer", CIF = "A12345678" };
        _customerServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdCustomer);

        // Act
        var result = await _controller.PostCustomer(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(CustomerController.GetCustomer));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.Value.Should().Be(createdCustomer);
    }

    [Fact]
    public async Task PutCustomer_ShouldReturnOk_WhenUpdateSuccessful()
    {
        // Arrange
        var updateDto = new UpdateCustomerDto { Name = "Updated", CIF = "B87654321" };
        var updatedCustomer = new CustomerDto { Id = 1, Name = "Updated", CIF = "B87654321" };
        _customerServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updatedCustomer);

        // Act
        var result = await _controller.PutCustomer(1, updateDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task PutCustomer_ShouldReturnNotFound_WhenCustomerNotFound()
    {
        // Arrange
        var updateDto = new UpdateCustomerDto { Name = "Updated", CIF = "B87654321" };
        _customerServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync((CustomerDto?)null);

        // Act
        var result = await _controller.PutCustomer(1, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnOk_WhenDeletionSuccessful()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCustomer(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnNotFound_WhenCustomerNotFound()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCustomer(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AssignKioskToCustomer_ShouldReturnOk_WhenAssignmentSuccessful()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.AssignKioskAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _controller.AssignKioskToCustomer(1, 2);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task AssignKioskToCustomer_ShouldReturnNotFound_WhenAssignmentFails()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.AssignKioskAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _controller.AssignKioskToCustomer(1, 2);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be("Customer or Kiosk not found.");
    }

    [Fact]
    public async Task UnassignKioskFromCustomer_ShouldReturnOk_WhenUnassignmentSuccessful()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.UnassignKioskAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _controller.UnassignKioskFromCustomer(1, 2);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task UnassignKioskFromCustomer_ShouldReturnNotFound_WhenUnassignmentFails()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.UnassignKioskAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _controller.UnassignKioskFromCustomer(1, 2);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be("Customer or Kiosk not found, or kiosk is not assigned to this customer.");
    }
}

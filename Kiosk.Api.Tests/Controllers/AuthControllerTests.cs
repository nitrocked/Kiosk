using FluentAssertions;
using Kiosk.Api.Controllers;
using Kiosk.Domain.DTOs.Auth;
using Kiosk.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kiosk.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithToken_WhenCredentialsValid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "admin", Password = "password" };
        var response = new LoginResponseDto { Token = "jwt-token", Expiry = DateTime.UtcNow.AddHours(1) };
        _authServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(response);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsInvalid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "invalid", Password = "invalid" };
        _authServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync((LoginResponseDto?)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.Value.Should().Be("Invalid credentials");
    }
}

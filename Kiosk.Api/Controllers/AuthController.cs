using Microsoft.AspNetCore.Mvc;
using Kiosk.Domain.DTOs.Auth;
using Kiosk.Domain.Interfaces;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    // POST: api/Auth/login
    /// <summary>
    /// Authenticates a user and returns a JWT token if successful. user: admin: password: password
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A JWT token if authentication is successful; otherwise, an unauthorized response.</returns>
    /// <response code="200">Returns the JWT token.</response>  
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized("Invalid credentials");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}: {Message}", loginDto.Username, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }
}
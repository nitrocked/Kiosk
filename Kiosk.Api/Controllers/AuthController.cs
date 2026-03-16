using Kiosk.Domain.DTOs.Auth;
using Kiosk.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
        {
            return Unauthorized("Invalid credentials");
        }
        return Ok(result);
    }
}
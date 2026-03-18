using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Kiosk.Domain.DTOs.Auth;
using Kiosk.Domain.Interfaces;

namespace Kiosk.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // For simplicity, using hardcoded credentials. In a real app, validate against database.
            if (loginDto.Username == "admin" && loginDto.Password == "password")
            {
                var token = GenerateJwtToken(loginDto.Username);
                return new LoginResponseDto
                {
                    Token = token,
                    Expiry = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "180")!)
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login Error for user {Username}: {Message}", loginDto.Username, ex.Message);
            throw new Exception($"Login Error: {ex.Message}");
        }
    }

    private string GenerateJwtToken(string username)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username)
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "180")!),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token Generation Error for user {Username}: {Message}", username, ex.Message);
            throw new Exception($"Token Generation Error: {ex.Message}");
        }
    }
}
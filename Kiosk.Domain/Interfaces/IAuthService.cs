using Kiosk.Domain.DTOs.Auth;

namespace Kiosk.Domain.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
}
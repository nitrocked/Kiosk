using Kiosk.Domain.DTOs;

namespace Kiosk.Domain.Interfaces;

public interface IKioskService
{
    Task<IEnumerable<KioskDto>> GetAllAsync();
    Task<KioskDto?> GetByIdAsync(int id);
    Task<KioskDto> CreateAsync(CreateKioskDto kioskDto);
    Task<KioskDto?> UpdateAsync(int id, UpdateKioskDto kioskDto);
    Task<bool> DeleteAsync(int id);
    
    // Relationship management
    Task<bool> AssignDeviceAsync(int kioskId, int deviceId);
    Task<bool> UnassignDeviceAsync(int kioskId, int deviceId);
}
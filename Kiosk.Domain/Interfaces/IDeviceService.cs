using Kiosk.Domain.DTOs;

namespace Kiosk.Domain.Interfaces;

public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<DeviceDto?> GetByIdAsync(int id);
    Task<DeviceDto> CreateAsync(CreateDeviceDto deviceDto);
    Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto deviceDto);
    Task<bool> DeleteAsync(int id);
}
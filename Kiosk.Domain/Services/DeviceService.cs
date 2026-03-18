using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using KioskEntities = Kiosk.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kiosk.Domain.Services;

public class DeviceService : IDeviceService
{
    private readonly KioskDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(KioskDbContext context, IMapper mapper, ILogger<DeviceService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        try
        {
            var devices = await _context.Devices
                .Include(d => d.Kiosk)
                    .ThenInclude(k => k.Customer)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DeviceDto>>(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices: {Message}", ex.Message);
            throw new Exception($"Error retrieving devices: {ex.Message}");
        }
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        try
        {
            var device = await _context.Devices
                .Include(d => d.Kiosk)
                    .ThenInclude(k => k.Customer)
                .FirstOrDefaultAsync(d => d.Id == id);

            return _mapper.Map<DeviceDto>(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving device with ID {DeviceId}: {Message}", id, ex.Message);
            throw new Exception($"Error retrieving device: {ex.Message}");
        }
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto deviceDto)
    {
        try
        {
            var device = _mapper.Map<KioskEntities.Device>(deviceDto);

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return _mapper.Map<DeviceDto>(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device: {Message}", ex.Message);
            throw new Exception($"Error creating device: {ex.Message}");
        }
    }

    public async Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto deviceDto)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null) return null;

            _mapper.Map(deviceDto, device);

            await _context.SaveChangesAsync();
            return _mapper.Map<DeviceDto>(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with ID {DeviceId}: {Message}", id, ex.Message);
            throw new Exception($"Error updating device: {ex.Message}");
        }
    }


    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null) return false;

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device with ID {DeviceId}: {Message}", id, ex.Message);
            throw new Exception($"Error deleting device: {ex.Message}");
        }
    }
}

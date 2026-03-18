using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using KioskEntities = Kiosk.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kiosk.Domain.Services;

public class KioskService : IKioskService
{
    private readonly KioskDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<KioskService> _logger;

    public KioskService(KioskDbContext context, IMapper mapper, ILogger<KioskService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<KioskDto>> GetAllAsync()
    {
        try
        {
            var kiosks = await _context.Kiosks
                .Include(k => k.Devices)
                .Include(k => k.Customer)
                .ToListAsync();

            return _mapper.Map<IEnumerable<KioskDto>>(kiosks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving kiosks: {Message}", ex.Message);
            throw new Exception($"Error retrieving kiosks: {ex.Message}");
        }
    }

    public async Task<KioskDto?> GetByIdAsync(int id)
    {
        try
        {
            var kiosk = await _context.Kiosks
                .Include(k => k.Devices)
                .Include(k => k.Customer)
                .FirstOrDefaultAsync(k => k.Id == id);

            return _mapper.Map<KioskDto>(kiosk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving kiosk with ID {KioskId}: {Message}", id, ex.Message);
            throw new Exception($"Error retrieving kiosk: {ex.Message}");
        }
    }

    public async Task<KioskDto> CreateAsync(CreateKioskDto kioskDto)
    {
        try
        {
            var kiosk = _mapper.Map<KioskEntities.Kiosk>(kioskDto);

            _context.Kiosks.Add(kiosk);
            await _context.SaveChangesAsync();

            return _mapper.Map<KioskDto>(kiosk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating kiosk: {Message}", ex.Message);
            throw new Exception($"Error creating kiosk: {ex.Message}");
        }
    }

    public async Task<KioskDto?> UpdateAsync(int id, UpdateKioskDto kioskDto)
    {
        try
        {
            var kiosk = await _context.Kiosks.FindAsync(id);
            if (kiosk == null) return null;

            _mapper.Map(kioskDto, kiosk);

            await _context.SaveChangesAsync();
            return _mapper.Map<KioskDto>(kiosk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating kiosk with ID {KioskId}: {Message}", id, ex.Message);
            throw new Exception($"Error updating kiosk: {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var kiosk = await _context.Kiosks.FindAsync(id);
            if (kiosk == null) return false;

            _context.Kiosks.Remove(kiosk);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting kiosk with ID {KioskId}: {Message}", id, ex.Message);
            throw new Exception($"Error deleting kiosk: {ex.Message}");
        }
    }

    public async Task<bool> AssignDeviceAsync(int kioskId, int deviceId)
    {
        try
        {
            var kiosk = await _context.Kiosks.FindAsync(kioskId);
            var device = await _context.Devices.FindAsync(deviceId);

            if (kiosk == null || device == null) return false;

            device.KioskId = kioskId;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning device with ID {DeviceId} to kiosk with ID {KioskId}: {Message}", deviceId, kioskId, ex.Message);
            throw new Exception($"Error assigning device: {ex.Message}");
        }
    }

    public async Task<bool> UnassignDeviceAsync(int kioskId, int deviceId)
    {
        try
        {
            var device = await _context.Devices.FindAsync(deviceId);

            if (device == null || device.KioskId != kioskId) return false;

            device.KioskId = null;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning device with ID {DeviceId} from kiosk with ID {KioskId}: {Message}", deviceId, kioskId, ex.Message);
            throw new Exception($"Error unassigning device: {ex.Message}");
        }
    }
}
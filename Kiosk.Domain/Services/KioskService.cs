using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using KioskEntities = Kiosk.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kiosk.Domain.Services;

public class KioskService : IKioskService
{
    private readonly KioskDbContext _context;
    private readonly IMapper _mapper;

    public KioskService(KioskDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<KioskDto>> GetAllAsync()
    {
        var kiosks = await _context.Kiosks
            .Include(k => k.Devices)
            .Include(k => k.Customer)
            .ToListAsync();

        return _mapper.Map<IEnumerable<KioskDto>>(kiosks);
    }

    public async Task<KioskDto?> GetByIdAsync(int id)
    {
        var kiosk = await _context.Kiosks
            .Include(k => k.Devices)
            .Include(k => k.Customer)
            .FirstOrDefaultAsync(k => k.Id == id);

        return _mapper.Map<KioskDto>(kiosk);
    }

    public async Task<KioskDto> CreateAsync(CreateKioskDto kioskDto)
    {
        var kiosk = _mapper.Map<KioskEntities.Kiosk>(kioskDto);

        _context.Kiosks.Add(kiosk);
        await _context.SaveChangesAsync();

        return _mapper.Map<KioskDto>(kiosk);
    }

    public async Task<KioskDto?> UpdateAsync(int id, UpdateKioskDto kioskDto)
    {
        var kiosk = await _context.Kiosks.FindAsync(id);
        if (kiosk == null) return null;

        _mapper.Map(kioskDto, kiosk);

        await _context.SaveChangesAsync();
        return _mapper.Map<KioskDto>(kiosk);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var kiosk = await _context.Kiosks.FindAsync(id);
        if (kiosk == null) return false;

        _context.Kiosks.Remove(kiosk);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignDeviceAsync(int kioskId, int deviceId)
    {
        var kiosk = await _context.Kiosks.FindAsync(kioskId);
        var device = await _context.Devices.FindAsync(deviceId);
        
        if (kiosk == null || device == null) return false;
        
        device.KioskId = kioskId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnassignDeviceAsync(int kioskId, int deviceId)
    {
        var device = await _context.Devices.FindAsync(deviceId);
        
        if (device == null || device.KioskId != kioskId) return false;
        
        device.KioskId = null;
        await _context.SaveChangesAsync();
        return true;
    }
}
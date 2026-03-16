using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using KioskEntities = Kiosk.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kiosk.Domain.Services;

public class DeviceService : IDeviceService
{
    private readonly KioskDbContext _context;
    private readonly IMapper _mapper;

    public DeviceService(KioskDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        var devices = await _context.Devices
            .Include(d => d.Kiosk)
#pragma warning disable CS8602
                .ThenInclude(k => k.Customer)
#pragma warning restore CS8602
            .ToListAsync();

        return _mapper.Map<IEnumerable<DeviceDto>>(devices);
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await _context.Devices
            .Include(d => d.Kiosk)
#pragma warning disable CS8602
                .ThenInclude(k => k.Customer)
#pragma warning restore CS8602
            .FirstOrDefaultAsync(d => d.Id == id);

        return _mapper.Map<DeviceDto>(device);
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto deviceDto)
    {
        var device = _mapper.Map<KioskEntities.Device>(deviceDto);

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return _mapper.Map<DeviceDto>(device);
    }

    public async Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto deviceDto)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return null;

        _mapper.Map(deviceDto, device);

        await _context.SaveChangesAsync();
        return _mapper.Map<DeviceDto>(device);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }
}
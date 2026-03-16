using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using KioskEntities = Kiosk.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kiosk.Domain.Services;

public class CustomerService : ICustomerService
{
    private readonly KioskDbContext _context;
    private readonly IMapper _mapper;

    public CustomerService(KioskDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _context.Customers
            .Include(c => c.Kiosks)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Kiosks)
                .ThenInclude(k => k.Devices)
            .FirstOrDefaultAsync(c => c.Id == id);

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto customerDto)
    {
        var customer = _mapper.Map<KioskEntities.Customer>(customerDto);

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto customerDto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return null;

        _mapper.Map(customerDto, customer);

        await _context.SaveChangesAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignKioskAsync(int customerId, int kioskId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        var kiosk = await _context.Kiosks.FindAsync(kioskId);
        
        if (customer == null || kiosk == null) return false;
        
        kiosk.CustomerId = customerId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnassignKioskAsync(int customerId, int kioskId)
    {
        var kiosk = await _context.Kiosks.FindAsync(kioskId);
        
        if (kiosk == null || kiosk.CustomerId != customerId) return false;
        
        kiosk.CustomerId = null;
        await _context.SaveChangesAsync();
        return true;
    }
}
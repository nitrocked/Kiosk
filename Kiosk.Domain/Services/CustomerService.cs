using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Interfaces;
using KioskEntities = Kiosk.Entities;
using AutoMapper;

namespace Kiosk.Domain.Services;

public class CustomerService : ICustomerService
{
    private readonly KioskDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(KioskDbContext context, IMapper mapper, ILogger<CustomerService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        try
        {
            var customers = await _context.Customers
                .Include(c => c.Kiosks)
                    .ThenInclude(k => k.Devices)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers: {Message}", ex.Message);
            throw new Exception($"Error retrieving customers: {ex.Message}");
        }
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        try
        {
            var customer = await _context.Customers
                .Include(c => c.Kiosks)
                    .ThenInclude(k => k.Devices)
                .FirstOrDefaultAsync(c => c.Id == id);

            return _mapper.Map<CustomerDto>(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}: {Message}", id, ex.Message);
            throw new Exception($"Error retrieving customer: {ex.Message}");
        }
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto customerDto)
    {
        try
        {
            var customer = _mapper.Map<KioskEntities.Customer>(customerDto);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {Message}", ex.Message);
            throw new Exception($"Error creating customer: {ex.Message}");
        }
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto customerDto)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return null;

            _mapper.Map(customerDto, customer);

            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID {CustomerId}: {Message}", id, ex.Message);
            throw new Exception($"Error updating customer: {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID {CustomerId}: {Message}", id, ex.Message);
            throw new Exception($"Error deleting customer: {ex.Message}");
        }
    }

    public async Task<bool> AssignKioskAsync(int customerId, int kioskId)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(customerId);
            var kiosk = await _context.Kiosks.FindAsync(kioskId);

            if (customer == null || kiosk == null) return false;

            kiosk.CustomerId = customerId;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning kiosk with ID {KioskId} to customer with ID {CustomerId}: {Message}", kioskId, customerId, ex.Message);
            throw new Exception($"Error assigning kiosk: {ex.Message}");
        }
    }

    public async Task<bool> UnassignKioskAsync(int customerId, int kioskId)
    {
        try
        {
            var kiosk = await _context.Kiosks.FindAsync(kioskId);

            if (kiosk == null || kiosk.CustomerId != customerId) return false;

            kiosk.CustomerId = null;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning kiosk with ID {KioskId} from customer with ID {CustomerId}: {Message}", kioskId, customerId, ex.Message);
            throw new Exception($"Error unassigning kiosk: {ex.Message}");
        }
    }
}
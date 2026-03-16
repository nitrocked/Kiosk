using Kiosk.Domain.DTOs;

namespace Kiosk.Domain.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CreateCustomerDto customerDto);
    Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto customerDto);
    Task<bool> DeleteAsync(int id);
    
    // Relationship management
    Task<bool> AssignKioskAsync(int customerId, int kioskId);
    Task<bool> UnassignKioskAsync(int customerId, int kioskId);
}
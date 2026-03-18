using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kiosk.Domain.Interfaces;
using Kiosk.Domain.DTOs;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;


    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    // GET: api/Customer
    /// <summary>
    /// Retrieves all customers along with their associated kiosks.
    /// </summary>
    /// <returns>A list of customers with their kiosks.</returns>
    /// <response code="200">Returns the list of customers.</response>
    /// <response code="500">If there was an error retrieving the customers.</response>
    [HttpGet]
    [Description("Retrieves all customers along with their associated kiosks.")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers: {Message}", ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // GET: api/Customer/5
    /// <summary>
    /// Retrieves a specific customer by ID, including their kiosks and devices.
    /// </summary>
    /// <param name="id">The ID of the customer to retrieve.</param>
    /// <returns>The customer with the specified ID, including their kiosks and devices.</returns>
    /// <response code="200">Returns the customer with the specified ID.</response>
    /// <response code="404">If the customer with the specified ID is not found.</response>
    /// <response code="500">If there was an error retrieving the customer.</response>
    [HttpGet("{id}")]
    [Description("Retrieves a specific customer by ID, including their kiosks and devices.")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}: {Message}", id, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // POST: api/Customer
    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="customerDto">The customer data to create.</param>
    /// <returns>The created customer.</returns>
    /// <response code="201">Returns the newly created customer.</response>
    /// <response code="400">If the customer data is invalid.</response>
    /// <response code="500">If there was an error creating the customer.</response>
    [HttpPost]
    [Description("Creates a new customer.")]
    public async Task<ActionResult<CustomerDto>> PostCustomer(CreateCustomerDto customerDto)
    {
        try
        {
            var createdCustomer = await _customerService.CreateAsync(customerDto);
            return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {Message}", ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // PUT: api/Customer/5
    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">The ID of the customer to update.</param>
    /// <param name="customerDto">The updated customer data.</param>
    /// <returns>Ok if the update was successful, or NotFound if the customer does not exist.</returns>
    /// <response code="200">If the customer was successfully updated.</response>
    /// <response code="404">If the customer with the specified ID is not found.</
    /// <response code="500">If there was an error updating the customer.</response>
    [HttpPut("{id}")]
    [Description("Updates an existing customer.")]
    public async Task<IActionResult> PutCustomer(int id, UpdateCustomerDto customerDto)
    {
        try
        {
            var updatedCustomer = await _customerService.UpdateAsync(id, customerDto);
            if (updatedCustomer == null)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID {CustomerId}: {Message}", id, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // DELETE: api/Customer/5
    /// <summary>
    /// Deletes a customer by ID.
    /// </summary>
    /// <param name="id">The ID of the customer to delete.</param>
    /// <returns>Ok if the deletion was successful, or NotFound if the customer does not exist.</returns>
    /// <response code="200">If the customer was successfully deleted.</response>
    /// <response code="404">If the customer with the specified ID is not found.</response>
    /// <response code="500">If there was an error deleting the customer.</response>
    [HttpDelete("{id}")]
    [Description("Deletes a customer by ID.")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var result = await _customerService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID {CustomerId}: {Message}", id, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // POST: api/Customer/{customerId}/kiosks/{kioskId}
    /// <summary>
    /// Assigns a kiosk to a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer to assign the kiosk to.</param>
    /// <param name="kioskId">The ID of the kiosk to assign.</param>
    /// <returns>Ok if the assignment was successful, or NotFound if the customer or kiosk does not exist.</returns>
    /// <response code="200">If the kiosk was successfully assigned to the customer.</response>
    /// <response code="404">If the customer or kiosk is not found.</response>
    [HttpPost("{customerId}/kiosks/{kioskId}")]
    [Description("Assigns a kiosk to a customer.")]
    public async Task<IActionResult> AssignKioskToCustomer(int customerId, int kioskId)
    {
        try
        {
            var result = await _customerService.AssignKioskAsync(customerId, kioskId);
            if (!result)
            {
                return NotFound("Customer or Kiosk not found.");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning kiosk with ID {KioskId} to customer with ID {CustomerId}: {Message}", kioskId, customerId, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // DELETE: api/Customer/{customerId}/kiosks/{kioskId}
    /// <summary>
    /// Unassigns a kiosk from a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer to unassign the kiosk from.</param>
    /// <param name="kioskId">The ID of the kiosk to unassign.</param>
    /// <returns>Ok if the unassignment was successful, or NotFound if the customer or kiosk does not exist, or if the kiosk is not assigned to this customer.</returns>    
    /// <response code="200">If the kiosk was successfully unassigned from the customer.</response>
    /// <response code="404">If the customer or kiosk is not found, or if
    [HttpDelete("{customerId}/kiosks/{kioskId}")]
    [Description("Unassigns a kiosk from a customer.")]
    public async Task<IActionResult> UnassignKioskFromCustomer(int customerId, int kioskId)
    {
        try
        {
            var result = await _customerService.UnassignKioskAsync(customerId, kioskId);
            if (!result)
            {
                return NotFound("Customer or Kiosk not found, or kiosk is not assigned to this customer.");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning kiosk with ID {KioskId} from customer with ID {CustomerId}: {Message}", kioskId, customerId, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }
}
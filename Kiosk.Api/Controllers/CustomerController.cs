using Microsoft.AspNetCore.Mvc;
using Kiosk.Domain.Interfaces;
using Kiosk.Domain.DTOs;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
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
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    // GET: api/Customer/5
    /// <summary>
    /// Retrieves a specific customer by ID, including their kiosks and devices.
    /// </summary>
    /// <param name="id">The ID of the customer to retrieve.</param>
    /// <returns>The customer with the specified ID, including their kiosks and devices.</returns>
    /// <response code="200">Returns the customer with the specified ID.</response>
    [HttpGet("{id}")]
    [Description("Retrieves a specific customer by ID, including their kiosks and devices.")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
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
        var createdCustomer = await _customerService.CreateAsync(customerDto);
        return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
    }

    // PUT: api/Customer/5
    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">The ID of the customer to update.</param>
    /// <param name="customerDto">The updated customer data.</param>
    /// <returns>No content if the update was successful, or NotFound if the customer does not exist.</returns>
    [HttpPut("{id}")]
    [Description("Updates an existing customer.")]
    public async Task<IActionResult> PutCustomer(int id, UpdateCustomerDto customerDto)
    {
        var updatedCustomer = await _customerService.UpdateAsync(id, customerDto);
        if (updatedCustomer == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Customer/5
    /// <summary>
    /// Deletes a customer by ID.
    /// </summary>
    /// <param name="id">The ID of the customer to delete.</param>
    /// <returns>No content if the deletion was successful, or NotFound if the customer does not exist.</returns>
    [HttpDelete("{id}")]
    [Description("Deletes a customer by ID.")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var result = await _customerService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Customer/{customerId}/kiosks/{kioskId}
    /// <summary>
    /// Assigns a kiosk to a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer to assign the kiosk to.</param>
    /// <param name="kioskId">The ID of the kiosk to assign.</param>
    /// <returns>No content if the assignment was successful, or NotFound if the customer or kiosk does not exist.</returns>
    [HttpPost("{customerId}/kiosks/{kioskId}")]
    [Description("Assigns a kiosk to a customer.")]    
    public async Task<IActionResult> AssignKioskToCustomer(int customerId, int kioskId)
    {
        var result = await _customerService.AssignKioskAsync(customerId, kioskId);
        if (!result)
        {
            return NotFound("Customer or Kiosk not found.");
        }

        return NoContent();
    }

    // DELETE: api/Customer/{customerId}/kiosks/{kioskId}
    /// <summary>
    /// Unassigns a kiosk from a customer.
    /// </summary>
    /// <param name="customerId">The ID of the customer to unassign the kiosk from.</param>
    /// <param name="kioskId">The ID of the kiosk to unassign.</param>
    /// <returns>No content if the unassignment was successful, or NotFound if the customer or kiosk does not exist, or if the kiosk is not assigned to this customer.</returns>    
    [HttpDelete("{customerId}/kiosks/{kioskId}")]
    [Description("Unassigns a kiosk from a customer.")]
    public async Task<IActionResult> UnassignKioskFromCustomer(int customerId, int kioskId)
    {
        var result = await _customerService.UnassignKioskAsync(customerId, kioskId);
        if (!result)
        {
            return NotFound("Customer or Kiosk not found, or kiosk is not assigned to this customer.");
        }

        return NoContent();
    }
}
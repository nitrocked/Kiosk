using Microsoft.AspNetCore.Mvc;
using Kiosk.Domain.Interfaces;
using Kiosk.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KioskController : ControllerBase
{
    private readonly IKioskService _kioskService;

    public KioskController(IKioskService kioskService)
    {
        _kioskService = kioskService;
    }

    // GET: api/Kiosk
    /// <summary>
    /// Retrieves all kiosks with their associated customer and devices.
    /// </summary>
    /// <returns>A list of all kiosks with their customer and device details.</returns>
    /// <response code="200">Returns the list of kiosks.</response>
    /// <response code="500">If there was an error retrieving the kiosks.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<KioskDto>>> GetKiosks()
    {
        var kiosks = await _kioskService.GetAllAsync();
        return Ok(kiosks);
    }

    // GET: api/Kiosk/5
    /// <summary>
    /// Retrieves a specific kiosk by ID, including its customer and devices.
    /// </summary>
    /// <param name="id">The ID of the kiosk to retrieve.</param>
    /// <returns>The kiosk with the specified ID.</returns>
    /// <response code="200">Returns the kiosk.</response>
    /// <response code="404">If the kiosk with the specified ID is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<KioskDto>> GetKiosk(int id)
    {
        var kiosk = await _kioskService.GetByIdAsync(id);

        if (kiosk == null)
        {
            return NotFound();
        }

        return kiosk;
    }

    // POST: api/Kiosk
    /// <summary>
    /// Creates a new kiosk (without assigning a customer or devices).
    /// </summary>
    /// <param name="kioskDto">The kiosk data to create.</param>
    /// <returns>The created kiosk with its ID.</returns>
    /// <response code="201">Returns the newly created kiosk.</response>
    /// <response code="400">If the kiosk data is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<KioskDto>> PostKiosk(CreateKioskDto kioskDto)
    {
        var createdKiosk = await _kioskService.CreateAsync(kioskDto);
        return CreatedAtAction(nameof(GetKiosk), new { id = createdKiosk.Id }, createdKiosk);
    }

    // PUT: api/Kiosk/5
    /// <summary>
    /// Updates an existing kiosk (without modifying its customer or device assignments).
    /// </summary>
    /// <param name="id">The ID of the kiosk to update.</param>
    /// <param name="kioskDto">The updated kiosk data.</param>
    /// <returns>Ok if the update was successful.</returns>
    /// <response code="200">If the kiosk was successfully updated.</response>
    /// <response code="404">If the kiosk with the specified ID is not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutKiosk(int id, UpdateKioskDto kioskDto)
    {
        var updatedKiosk = await _kioskService.UpdateAsync(id, kioskDto);
        if (updatedKiosk == null)
        {
            return NotFound();
        }

        return Ok();
    }

    // DELETE: api/Kiosk/5
    /// <summary>
    /// Deletes a kiosk by ID.
    /// </summary>
    /// <param name="id">The ID of the kiosk to delete.</param>
    /// <returns>Ok if the deletion was successful.</returns>
    /// <response code="200">If the kiosk was successfully deleted.</response>
    /// <response code="404">If the kiosk with the specified ID is not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKiosk(int id)
    {
        var result = await _kioskService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }

    // POST: api/Kiosk/{kioskId}/devices/{deviceId}
    /// <summary>
    /// Assigns a device to a kiosk.
    /// </summary>
    /// <param name="kioskId">The ID of the kiosk to assign the device to.</param>
    /// <param name="deviceId">The ID of the device to assign.</param>
    /// <returns>Ok if the assignment was successful, or NotFound if the kiosk or device does not exist.</returns>
    /// <response code="200">If the device was successfully assigned to the kiosk.</response>
    /// <response code="404">If the kiosk or device is not found.</response>
    [HttpPost("{kioskId}/devices/{deviceId}")]
    public async Task<IActionResult> AssignDeviceToKiosk(int kioskId, int deviceId)
    {
        var result = await _kioskService.AssignDeviceAsync(kioskId, deviceId);
        if (!result)
        {
            return NotFound("Kiosk or Device not found.");
        }

        return Ok();
    }

    // DELETE: api/Kiosk/{kioskId}/devices/{deviceId}
    /// <summary>
    /// Unassigns a device from a kiosk.
    /// </summary>
    /// <param name="kioskId">The ID of the kiosk to unassign the device from.</param>
    /// <param name="deviceId">The ID of the device to unassign.</param>
    /// <returns>Ok if the unassignment was successful, or NotFound if the kiosk or device does not exist, or device is not assigned to this kiosk.</returns>
    /// <response code="200">If the device was successfully unassigned from the kiosk.</response>
    /// <response code="404">If the kiosk or device is not found, or device is not assigned to this kiosk.</response>
    [HttpDelete("{kioskId}/devices/{deviceId}")]
    public async Task<IActionResult> UnassignDeviceFromKiosk(int kioskId, int deviceId)
    {
        var result = await _kioskService.UnassignDeviceAsync(kioskId, deviceId);
        if (!result)
        {
            return NotFound("Kiosk or Device not found, or device is not assigned to this kiosk.");
        }

        return Ok();
    }
}
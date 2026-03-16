using Microsoft.AspNetCore.Mvc;
using Kiosk.Domain.Interfaces;
using Kiosk.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    // GET: api/Device
    /// <summary>
    /// Retrieves all devices.
    /// </summary>
    /// <returns>A list of all devices.</returns>
    /// <response code="200">Returns the list of devices.</response>
    /// <response code="500">If there was an error retrieving the devices.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevices()
    {
        var devices = await _deviceService.GetAllAsync();
        return Ok(devices);
    }

    // GET: api/Device/5
    /// <summary>
    /// Retrieves a specific device by ID.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <returns>The device with the specified ID.</returns>
    /// <response code="200">Returns the device.</response>
    /// <response code="404">If the device with the specified ID is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetDevice(int id)
    {
        var device = await _deviceService.GetByIdAsync(id);

        if (device == null)
        {
            return NotFound();
        }

        return device;
    }

    // POST: api/Device
    /// <summary>
    /// Creates a new device.
    /// </summary>
    /// <param name="deviceDto">The device data to create.</param>
    /// <returns>The created device with its ID.</returns>
    /// <response code="201">Returns the newly created device.</response>
    /// <response code="400">If the device data is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<DeviceDto>> PostDevice(CreateDeviceDto deviceDto)
    {
        var createdDevice = await _deviceService.CreateAsync(deviceDto);
        return CreatedAtAction(nameof(GetDevice), new { id = createdDevice.Id }, createdDevice);
    }

    // PUT: api/Device/5
    /// <summary>
    /// Updates an existing device (without modifying its kiosk assignment).
    /// </summary>
    /// <param name="id">The ID of the device to update.</param>
    /// <param name="deviceDto">The updated device data.</param>
    /// <returns>No content if the update was successful.</returns>
    /// <response code="204">If the device was successfully updated.</response>
    /// <response code="404">If the device with the specified ID is not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDevice(int id, UpdateDeviceDto deviceDto)
    {
        var updatedDevice = await _deviceService.UpdateAsync(id, deviceDto);
        if (updatedDevice == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Device/5
    /// <summary>
    /// Deletes a device by ID.
    /// </summary>
    /// <param name="id">The ID of the device to delete.</param>
    /// <returns>No content if the deletion was successful.</returns>
    /// <response code="204">If the device was successfully deleted.</response>
    /// <response code="404">If the device with the specified ID is not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        var result = await _deviceService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
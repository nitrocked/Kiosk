using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kiosk.Domain.Interfaces;
using Kiosk.Domain.DTOs;

namespace Kiosk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger<DeviceController> _logger;


    public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger)
    {
        _deviceService = deviceService;
        _logger = logger;

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
        try
        {
            var devices = await _deviceService.GetAllAsync();
            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices: {Message}", ex.Message);
            return StatusCode(500, $"Internal server error");
        }
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
        try
        {
            var device = await _deviceService.GetByIdAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving device with ID {DeviceId}: {Message}", id, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
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
        try
        {
            var createdDevice = await _deviceService.CreateAsync(deviceDto);
            return CreatedAtAction(nameof(GetDevice), new { id = createdDevice.Id }, createdDevice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device: {Message}", ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // PUT: api/Device/5
    /// <summary>
    /// Updates an existing device (without modifying its kiosk assignment).
    /// </summary>
    /// <param name="id">The ID of the device to update.</param>
    /// <param name="deviceDto">The updated device data.</param>
    /// <returns>Ok if the update was successful.</returns>
    /// <response code="200">If the device was successfully updated.</response>
    /// <response code="404">If the device with the specified ID is not found.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDevice(int id, UpdateDeviceDto deviceDto)
    {
        try
        {
            var updatedDevice = await _deviceService.UpdateAsync(id, deviceDto);
            if (updatedDevice == null)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with ID {DeviceId}: {Message}", id, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }

    // DELETE: api/Device/5
    /// <summary>
    /// Deletes a device by ID.
    /// </summary>
    /// <param name="id">The ID of the device to delete.</param>
    /// <returns>OK if the deletion was successful.</returns>
    /// <response code="200">If the device was successfully deleted.</response>
    /// <response code="404">If the device with the specified ID is not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        try
        {
            var result = await _deviceService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device with ID {DeviceId}: {Message}", id, ex.Message);
            return StatusCode(500, $"Internal server error");
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Kiosk.Domain.DTOs;

public class DeviceDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Device name is required.")]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    [Required]
    public string DeviceType { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    public string? FirmwareVersion { get; set; }

    public int? KioskId { get; set; }

    public KioskDto? Kiosk { get; set; }
}
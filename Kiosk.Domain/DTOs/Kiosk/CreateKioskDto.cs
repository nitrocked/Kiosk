using System.ComponentModel.DataAnnotations;

namespace Kiosk.Domain.DTOs;

public class CreateKioskDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    [Url]
    public string? AdminURL { get; set; }

    [Range(1, 65535, ErrorMessage = "Port must be a valid number.")]
    public int? AdminPort { get; set; }

    public string? Version { get; set; }

    public string? Location { get; set; }
}
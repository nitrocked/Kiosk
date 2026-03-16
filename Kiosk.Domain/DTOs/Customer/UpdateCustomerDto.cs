using System.ComponentModel.DataAnnotations;

namespace Kiosk.Domain.DTOs;

public class UpdateCustomerDto
{
    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "CIF is required.")]
    [RegularExpression("^[A-Z][0-9]{8}$", ErrorMessage = "CIF must start with a capital letter followed by 8 digits.")]
    public string CIF { get; set; } = string.Empty;
}
namespace Kiosk.Entities;

public class Device
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? SerialNumber { get; set; }
    public string? DeviceType { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? FirmwareVersion { get; set; }

    // Relationships
    public int? KioskId { get; set; }
    public virtual Kiosk? Kiosk { get; set; }
}
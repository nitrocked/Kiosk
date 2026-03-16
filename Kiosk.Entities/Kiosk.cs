namespace Kiosk.Entities;

public class Kiosk
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? SerialNumber { get; set; }
    public required string AdminURL { get; set; }
    public required int AdminPort { get; set; }
    public string? Version { get; set; }
    public string? Location { get; set; }

    // Relationships
    public int? CustomerId { get; set; }
    public virtual Customer? Customer { get; set; } = null;
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}

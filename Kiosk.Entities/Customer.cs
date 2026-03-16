namespace Kiosk.Entities;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? CIF { get; set; }

    // Relationships
    public virtual ICollection<Kiosk> Kiosks { get; set; } = new List<Kiosk>();
}
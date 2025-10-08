namespace DroneComply.Core.Models;

public class AircraftEquipment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime InstalledOn { get; set; } = DateTime.UtcNow;
    public DateTime? LastServicedOn { get; set; }
}

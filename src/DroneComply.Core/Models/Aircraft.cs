using DroneComply.Core.Enums;

namespace DroneComply.Core.Models;

public class Aircraft
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public AircraftType Type { get; set; } = AircraftType.Multirotor;
    public double MaxTakeoffWeightKg { get; set; }
    public double MaxFlightTimeMinutes { get; set; }
    public AircraftStatus Status { get; set; } = AircraftStatus.Active;
    public DateTime CommissionedOn { get; set; } = DateTime.UtcNow;
    public DateTime? LastMaintenance { get; set; }
    public List<AircraftEquipment> Equipment { get; set; } = new();
}

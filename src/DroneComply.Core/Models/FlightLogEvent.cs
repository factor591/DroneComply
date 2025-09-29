namespace DroneComply.Core.Models;

public class FlightLogEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FlightLogId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

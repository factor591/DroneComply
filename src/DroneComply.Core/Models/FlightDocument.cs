namespace DroneComply.Core.Models;

public class FlightDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FlightLogId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
}

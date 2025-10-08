namespace DroneComply.Core.Models;

public class WeatherAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WeatherBriefingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Effective { get; set; } = DateTime.UtcNow;
    public DateTime Expires { get; set; } = DateTime.UtcNow;
}

namespace DroneComply.Core.Models;

public class WeatherBriefing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MissionPlanId { get; set; }
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<WeatherCondition> Conditions { get; set; } = new();
    public List<WeatherAlert> Alerts { get; set; } = new();
}

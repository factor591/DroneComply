namespace DroneComply.Core.Models;

public class WeatherCondition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WeatherBriefingId { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime ObservationTime { get; set; } = DateTime.UtcNow;
    public double TemperatureCelsius { get; set; }
    public double WindSpeedKnots { get; set; }
    public double WindGustKnots { get; set; }
    public int WindDirectionDegrees { get; set; }
    public double VisibilityMiles { get; set; }
    public double CeilingFeet { get; set; }
    public string WeatherPhenomena { get; set; } = string.Empty;
}

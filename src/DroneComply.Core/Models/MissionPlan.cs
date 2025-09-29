using DroneComply.Core.Enums;

namespace DroneComply.Core.Models;

public class MissionPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid PilotId { get; set; }
    public Guid AircraftId { get; set; }
    public DateTime PlannedDate { get; set; } = DateTime.UtcNow;
    public string LaunchLocation { get; set; } = string.Empty;
    public string LandingLocation { get; set; } = string.Empty;
    public MissionStatus Status { get; set; } = MissionStatus.Draft;
    public FlightRiskLevel RiskLevel { get; set; } = FlightRiskLevel.Low;
    public string MissionObjective { get; set; } = string.Empty;
    public List<MissionChecklistItem> Checklist { get; set; } = new();
    public List<MissionAirspaceAdvisory> AirspaceAdvisories { get; set; } = new();
    public WeatherBriefing? WeatherBriefing { get; set; }
}

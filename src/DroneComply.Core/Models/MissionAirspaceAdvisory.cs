namespace DroneComply.Core.Models;

public class MissionAirspaceAdvisory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MissionPlanId { get; set; }
    public string AdvisoryId { get; set; } = string.Empty;
    public string AdvisoryType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime EffectiveTo { get; set; } = DateTime.UtcNow;
    public string Severity { get; set; } = string.Empty;
}

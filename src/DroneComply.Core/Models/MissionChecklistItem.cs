namespace DroneComply.Core.Models;

public class MissionChecklistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MissionPlanId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string Notes { get; set; } = string.Empty;
}

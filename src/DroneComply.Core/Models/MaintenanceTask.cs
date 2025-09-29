namespace DroneComply.Core.Models;

public class MaintenanceTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MaintenanceRecordId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string Notes { get; set; } = string.Empty;
}

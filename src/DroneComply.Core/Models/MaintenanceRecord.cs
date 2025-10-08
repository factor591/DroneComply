using System;
using System.Collections.Generic;
using DroneComply.Core.Enums;

namespace DroneComply.Core.Models;

public class MaintenanceRecord : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AircraftId { get; set; }
    public MaintenanceType Type { get; set; } = MaintenanceType.Scheduled;
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<MaintenanceTask> Tasks { get; set; } = new();
}

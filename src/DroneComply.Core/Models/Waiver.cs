using System;
using System.Collections.Generic;
using DroneComply.Core.Enums;

namespace DroneComply.Core.Models;

public class Waiver : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public WaiverType Type { get; set; } = WaiverType.Part107_200;
    public WaiverStatus Status { get; set; } = WaiverStatus.Draft;
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedOn { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<WaiverCondition> Conditions { get; set; } = new();
}

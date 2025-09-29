using System;
using System.Collections.Generic;

namespace DroneComply.Core.Models;

public class ComplianceReport : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MissionPlanId { get; set; }
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public string Summary { get; set; } = string.Empty;
    public List<ComplianceViolation> Violations { get; set; } = new();
    public bool IsCompliant => Violations.Count == 0;
}

namespace DroneComply.Core.Models;

public class ComplianceViolation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ComplianceReportId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}

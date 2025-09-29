using DroneComply.Core.Enums;

namespace DroneComply.Core.Models;

public class PilotCertification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public CertificationType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime IssuedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresOn { get; set; }
}

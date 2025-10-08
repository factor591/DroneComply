namespace DroneComply.Core.Models;

public class Pilot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime CertificationDate { get; set; } = DateTime.UtcNow;
    public DateTime? RecurrentTrainingDue { get; set; }
    public bool IsActive { get; set; } = true;
    public List<PilotCertification> Certifications { get; set; } = new();
    public List<PilotCredential> Credentials { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}".Trim();
}

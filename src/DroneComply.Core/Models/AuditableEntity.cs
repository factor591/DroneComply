using System;

namespace DroneComply.Core.Models;

public abstract class AuditableEntity
{
    private const string SystemUser = "system";

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string CreatedBy { get; private set; } = SystemUser;
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    public void MarkCreated(string? userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = NormalizeUser(userId);
        ModifiedAt = null;
        ModifiedBy = null;
    }

    public void MarkModified(string? userId)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = NormalizeUser(userId);
    }

    private static string NormalizeUser(string? userId)
    {
        return string.IsNullOrWhiteSpace(userId) ? SystemUser : userId.Trim();
    }
}

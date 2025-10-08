using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IPilotRepository : IAsyncRepository<Pilot>
{
    Task<IReadOnlyList<Pilot>> GetActivePilotsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pilot>> GetPilotsWithExpiringCertificationsAsync(int daysThreshold, CancellationToken cancellationToken = default);
}

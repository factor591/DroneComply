using DroneComply.Core.Enums;
using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IAircraftRepository : IAsyncRepository<Aircraft>
{
    Task<IReadOnlyList<Aircraft>> GetByStatusAsync(AircraftStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Aircraft>> GetActiveAircraftAsync(CancellationToken cancellationToken = default);
}

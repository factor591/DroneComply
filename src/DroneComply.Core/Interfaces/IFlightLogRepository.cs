using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IFlightLogRepository : IAsyncRepository<FlightLog>
{
    Task<IReadOnlyList<FlightLog>> GetForPilotAsync(Guid pilotId, CancellationToken cancellationToken = default);
    Task<FlightLog?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IFlightLogService
{
    Task<FlightLog> CreateAsync(FlightLog log, CancellationToken cancellationToken = default);
    Task<FlightLog?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FlightLog>> GetForPilotAsync(Guid pilotId, CancellationToken cancellationToken = default);
    Task UpdateAsync(FlightLog log, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

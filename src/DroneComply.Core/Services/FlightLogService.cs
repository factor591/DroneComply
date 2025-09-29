using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.Core.Services;

public class FlightLogService : IFlightLogService
{
    private readonly IFlightLogRepository _flightLogRepository;

    public FlightLogService(IFlightLogRepository flightLogRepository)
    {
        _flightLogRepository = flightLogRepository;
    }

    public Task<FlightLog> CreateAsync(FlightLog log, CancellationToken cancellationToken = default)
    {
        log.Id = log.Id == Guid.Empty ? Guid.NewGuid() : log.Id;
        return _flightLogRepository.AddAsync(log, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _flightLogRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<FlightLog?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _flightLogRepository.GetWithDetailsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<FlightLog>> GetForPilotAsync(Guid pilotId, CancellationToken cancellationToken = default)
    {
        return _flightLogRepository.GetForPilotAsync(pilotId, cancellationToken);
    }

    public Task UpdateAsync(FlightLog log, CancellationToken cancellationToken = default)
    {
        return _flightLogRepository.UpdateAsync(log, cancellationToken);
    }
}

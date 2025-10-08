using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IAviationWeatherService
{
    Task<MetarData?> GetMetarAsync(string stationId, CancellationToken cancellationToken = default);
    Task<TafData?> GetTafAsync(string stationId, CancellationToken cancellationToken = default);
}

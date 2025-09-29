using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces.External;

public interface IWeatherService
{
    Task<WeatherBriefing> GetWeatherBriefingAsync(MissionPlan missionPlan, CancellationToken cancellationToken = default);
}

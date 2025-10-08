using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IMissionPlanRepository : IAsyncRepository<MissionPlan>
{
    Task<MissionPlan?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MissionPlan>> GetUpcomingAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MissionAirspaceAdvisory>> ReplaceAirspaceAdvisoriesAsync(Guid missionPlanId, IEnumerable<MissionAirspaceAdvisory> advisories, CancellationToken cancellationToken = default);
    Task<WeatherBriefing> ReplaceWeatherBriefingAsync(Guid missionPlanId, WeatherBriefing briefing, CancellationToken cancellationToken = default);
}

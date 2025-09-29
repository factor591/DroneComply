using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IPreFlightService
{
    Task<MissionPlan> CreateMissionPlanAsync(MissionPlan plan, CancellationToken cancellationToken = default);
    Task<ComplianceReport> EvaluateComplianceAsync(Guid missionPlanId, CancellationToken cancellationToken = default);
    Task<WeatherBriefing> RefreshWeatherBriefingAsync(Guid missionPlanId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MissionAirspaceAdvisory>> RefreshAirspaceAdvisoriesAsync(Guid missionPlanId, CancellationToken cancellationToken = default);
}

using System.Collections.Generic;
using DroneComply.Core.Models;
using DroneComply.Core.Primitives;

namespace DroneComply.Core.Interfaces;

public interface IPreFlightService
{
    Task<Result<MissionPlan>> CreateMissionPlanAsync(MissionPlan plan, CancellationToken cancellationToken = default);
    Task<Result<ComplianceReport>> EvaluateComplianceAsync(Guid missionPlanId, CancellationToken cancellationToken = default);
    Task<Result<WeatherBriefing>> RefreshWeatherBriefingAsync(Guid missionPlanId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<MissionAirspaceAdvisory>>> RefreshAirspaceAdvisoriesAsync(Guid missionPlanId, CancellationToken cancellationToken = default);
}

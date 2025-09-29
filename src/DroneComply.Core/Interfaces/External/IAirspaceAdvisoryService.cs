using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces.External;

public interface IAirspaceAdvisoryService
{
    Task<IReadOnlyList<MissionAirspaceAdvisory>> GetAirspaceAdvisoriesAsync(MissionPlan missionPlan, CancellationToken cancellationToken = default);
}

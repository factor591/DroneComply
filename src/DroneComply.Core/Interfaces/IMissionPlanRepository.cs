using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IMissionPlanRepository : IAsyncRepository<MissionPlan>
{
    Task<MissionPlan?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MissionPlan>> GetUpcomingAsync(int count, CancellationToken cancellationToken = default);
}

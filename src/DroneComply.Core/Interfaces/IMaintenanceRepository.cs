using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IMaintenanceRepository : IAsyncRepository<MaintenanceRecord>
{
    Task<IReadOnlyList<MaintenanceRecord>> GetOutstandingAsync(Guid aircraftId, CancellationToken cancellationToken = default);
    Task<MaintenanceRecord?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MaintenanceTask?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task UpdateTaskAsync(MaintenanceTask task, CancellationToken cancellationToken = default);
}

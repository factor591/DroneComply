using DroneComply.Core.Models;

namespace DroneComply.Core.Interfaces;

public interface IMaintenanceService
{
    Task<MaintenanceRecord> ScheduleAsync(MaintenanceRecord record, CancellationToken cancellationToken = default);
    Task CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MaintenanceRecord>> GetOutstandingAsync(Guid aircraftId, CancellationToken cancellationToken = default);
}

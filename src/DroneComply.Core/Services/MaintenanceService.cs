using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.Core.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenanceRepository;

    public MaintenanceService(IMaintenanceRepository maintenanceRepository)
    {
        _maintenanceRepository = maintenanceRepository;
    }

    public Task<MaintenanceRecord> ScheduleAsync(MaintenanceRecord record, CancellationToken cancellationToken = default)
    {
        record.Id = record.Id == Guid.Empty ? Guid.NewGuid() : record.Id;
        record.Status = MaintenanceStatus.Scheduled;
        return _maintenanceRepository.AddAsync(record, cancellationToken);
    }

    public async Task CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _maintenanceRepository.GetTaskAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Maintenance task {taskId} was not found.");

        if (!task.IsCompleted)
        {
            task.IsCompleted = true;
            task.CompletedOn = DateTime.UtcNow;
            await _maintenanceRepository.UpdateTaskAsync(task, cancellationToken);
        }

        var record = await _maintenanceRepository.GetWithDetailsAsync(task.MaintenanceRecordId, cancellationToken);
        if (record is null)
        {
            return;
        }

        if (record.Tasks.All(t => t.IsCompleted))
        {
            record.Status = MaintenanceStatus.Completed;
            record.CompletedDate = DateTime.UtcNow;
            await _maintenanceRepository.UpdateAsync(record, cancellationToken);
        }
    }

    public Task<IReadOnlyList<MaintenanceRecord>> GetOutstandingAsync(Guid aircraftId, CancellationToken cancellationToken = default)
    {
        return _maintenanceRepository.GetOutstandingAsync(aircraftId, cancellationToken);
    }
}

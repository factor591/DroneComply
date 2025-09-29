using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DroneComply.Data.Repositories;

public class MaintenanceRepository : EfRepository<MaintenanceRecord>, IMaintenanceRepository
{
    public MaintenanceRepository(DroneComplyDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<MaintenanceRecord>> GetOutstandingAsync(Guid aircraftId, CancellationToken cancellationToken = default)
    {
        return await DbContext.MaintenanceRecords
            .Include(record => record.Tasks)
            .Where(record => record.AircraftId == aircraftId && record.Status != MaintenanceStatus.Completed)
            .OrderBy(record => record.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MaintenanceRecord?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.MaintenanceRecords
            .Include(record => record.Tasks)
            .FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
    }

    public async Task<MaintenanceTask?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await DbContext.MaintenanceTasks
            .FirstOrDefaultAsync(task => task.Id == taskId, cancellationToken);
    }

    public async Task UpdateTaskAsync(MaintenanceTask task, CancellationToken cancellationToken = default)
    {
        DbContext.MaintenanceTasks.Update(task);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}

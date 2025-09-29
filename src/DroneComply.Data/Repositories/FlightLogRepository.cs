using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DroneComply.Data.Repositories;

public class FlightLogRepository : EfRepository<FlightLog>, IFlightLogRepository
{
    public FlightLogRepository(DroneComplyDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<FlightLog>> GetForPilotAsync(Guid pilotId, CancellationToken cancellationToken = default)
    {
        return await DbContext.FlightLogs
            .AsNoTracking()
            .Where(log => log.PilotId == pilotId)
            .OrderByDescending(log => log.FlightDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<FlightLog?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.FlightLogs
            .Include(log => log.Events)
            .Include(log => log.Attachments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(log => log.Id == id, cancellationToken);
    }
}

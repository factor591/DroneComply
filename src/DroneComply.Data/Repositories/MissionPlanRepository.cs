using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DroneComply.Data.Repositories;

public class MissionPlanRepository : EfRepository<MissionPlan>, IMissionPlanRepository
{
    public MissionPlanRepository(DroneComplyDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<MissionPlan?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.MissionPlans
            .Include(p => p.Checklist)
            .Include(p => p.AirspaceAdvisories)
            .Include(p => p.WeatherBriefing)
                .ThenInclude(b => b!.Conditions)
            .Include(p => p.WeatherBriefing)
                .ThenInclude(b => b!.Alerts)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MissionPlan>> GetUpcomingAsync(int count, CancellationToken cancellationToken = default)
    {
        return await DbContext.MissionPlans
            .AsNoTracking()
            .Where(p => p.PlannedDate >= DateTime.UtcNow.AddDays(-1))
            .OrderBy(p => p.PlannedDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}

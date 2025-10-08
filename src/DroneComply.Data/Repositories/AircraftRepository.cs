using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DroneComply.Data.Repositories;

public class AircraftRepository : EfRepository<Aircraft>, IAircraftRepository
{
    public AircraftRepository(DroneComplyDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IReadOnlyList<Aircraft>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Aircraft
            .Include(a => a.Equipment)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Aircraft>> GetByStatusAsync(AircraftStatus status, CancellationToken cancellationToken = default)
    {
        return await DbContext.Aircraft
            .Where(a => a.Status == status)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Aircraft>> GetActiveAircraftAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Aircraft
            .Where(a => a.Status == AircraftStatus.Active)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }
}

using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using DroneComply.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DroneComply.Data.Repositories;

public class PilotRepository : EfRepository<Pilot>, IPilotRepository
{
    public PilotRepository(DroneComplyDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IReadOnlyList<Pilot>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Pilots
            .Include(p => p.Certifications)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pilot>> GetActivePilotsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Pilots
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pilot>> GetPilotsWithExpiringCertificationsAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

        return await DbContext.Pilots
            .Where(p => p.IsActive && p.Certifications.Any(c => c.ExpiresOn.HasValue && c.ExpiresOn.Value <= thresholdDate))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }
}

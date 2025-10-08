using System.Collections.Generic;
using System.Linq;
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

    public async Task<IReadOnlyList<MissionAirspaceAdvisory>> ReplaceAirspaceAdvisoriesAsync(
        Guid missionPlanId,
        IEnumerable<MissionAirspaceAdvisory> advisories,
        CancellationToken cancellationToken = default)
    {
        DbContext.ChangeTracker.Clear();

        await DbContext.MissionAirspaceAdvisories
            .Where(a => a.MissionPlanId == missionPlanId)
            .ExecuteDeleteAsync(cancellationToken);

        var normalized = advisories
            .Select(advisory => new MissionAirspaceAdvisory
            {
                Id = advisory.Id == Guid.Empty ? Guid.NewGuid() : advisory.Id,
                MissionPlanId = missionPlanId,
                AdvisoryId = advisory.AdvisoryId,
                AdvisoryType = advisory.AdvisoryType,
                Description = advisory.Description,
                EffectiveFrom = advisory.EffectiveFrom,
                EffectiveTo = advisory.EffectiveTo,
                Severity = advisory.Severity
            })
            .ToList();

        if (normalized.Count > 0)
        {
            await DbContext.MissionAirspaceAdvisories.AddRangeAsync(normalized, cancellationToken);
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return normalized;
    }

    public async Task<WeatherBriefing> ReplaceWeatherBriefingAsync(
        Guid missionPlanId,
        WeatherBriefing briefing,
        CancellationToken cancellationToken = default)
    {
        DbContext.ChangeTracker.Clear();

        var existingBriefingIds = await DbContext.WeatherBriefings
            .Where(b => b.MissionPlanId == missionPlanId)
            .Select(b => b.Id)
            .ToListAsync(cancellationToken);

        if (existingBriefingIds.Count > 0)
        {
            await DbContext.WeatherAlerts
                .Where(a => existingBriefingIds.Contains(a.WeatherBriefingId))
                .ExecuteDeleteAsync(cancellationToken);

            await DbContext.WeatherConditions
                .Where(c => existingBriefingIds.Contains(c.WeatherBriefingId))
                .ExecuteDeleteAsync(cancellationToken);

            await DbContext.WeatherBriefings
                .Where(b => existingBriefingIds.Contains(b.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }

        var normalized = new WeatherBriefing
        {
            Id = briefing.Id == Guid.Empty ? Guid.NewGuid() : briefing.Id,
            MissionPlanId = missionPlanId,
            RetrievedAt = briefing.RetrievedAt,
            Source = briefing.Source,
            Summary = briefing.Summary
        };

        normalized.Conditions = briefing.Conditions
            .Select(condition => new WeatherCondition
            {
                Id = condition.Id == Guid.Empty ? Guid.NewGuid() : condition.Id,
                WeatherBriefingId = normalized.Id,
                Location = condition.Location,
                ObservationTime = condition.ObservationTime,
                TemperatureCelsius = condition.TemperatureCelsius,
                WindSpeedKnots = condition.WindSpeedKnots,
                WindGustKnots = condition.WindGustKnots,
                WindDirectionDegrees = condition.WindDirectionDegrees,
                VisibilityMiles = condition.VisibilityMiles,
                CeilingFeet = condition.CeilingFeet,
                WeatherPhenomena = condition.WeatherPhenomena
            })
            .ToList();

        normalized.Alerts = briefing.Alerts
            .Select(alert => new WeatherAlert
            {
                Id = alert.Id == Guid.Empty ? Guid.NewGuid() : alert.Id,
                WeatherBriefingId = normalized.Id,
                Title = alert.Title,
                Severity = alert.Severity,
                Description = alert.Description,
                Effective = alert.Effective,
                Expires = alert.Expires
            })
            .ToList();

        await DbContext.WeatherBriefings.AddAsync(normalized, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return normalized;
    }
}

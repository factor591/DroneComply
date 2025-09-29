using System;
using DroneComply.Core.Interfaces.External;
using DroneComply.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DroneComply.External.Airspace;

public class AloftAirspaceService : IAirspaceAdvisoryService
{
    private readonly ILogger<AloftAirspaceService> _logger;
    private readonly string _laancKey;
    private readonly string _laancKeyVariable;

    public AloftAirspaceService(IConfiguration configuration, ILogger<AloftAirspaceService> logger)
    {
        _logger = logger;
        _laancKeyVariable = configuration["Secrets:ApiKeys:Aloft"] ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(_laancKeyVariable))
        {
            _laancKey = Environment.GetEnvironmentVariable(_laancKeyVariable) ?? string.Empty;
        }
        else
        {
            _laancKey = configuration["ExternalAPIs:AloftAPI"] ?? string.Empty;
        }
    }

    public Task<IReadOnlyList<MissionAirspaceAdvisory>> GetAirspaceAdvisoriesAsync(MissionPlan missionPlan, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_laancKey))
        {
            if (!string.IsNullOrWhiteSpace(_laancKeyVariable))
            {
                _logger.LogWarning("Environment variable {EnvVar} is not set. Returning offline advisories for mission {MissionPlan}", _laancKeyVariable, missionPlan.Id);
            }
            else
            {
                _logger.LogInformation("Aloft API key not configured. Returning offline advisories for mission {MissionPlan}", missionPlan.Id);
            }
        }

        var advisories = new List<MissionAirspaceAdvisory>
        {
            new()
            {
                MissionPlanId = missionPlan.Id,
                AdvisoryId = "LAANC-PRECHECK",
                AdvisoryType = "Pre-authorization",
                Description = "Mission requires LAANC authorization before flight in controlled airspace.",
                EffectiveFrom = missionPlan.PlannedDate.AddHours(-2),
                EffectiveTo = missionPlan.PlannedDate.AddHours(2),
                Severity = "Moderate"
            }
        };

        if (missionPlan.PlannedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            advisories.Add(new MissionAirspaceAdvisory
            {
                MissionPlanId = missionPlan.Id,
                AdvisoryId = "AIRSPACE-STAFFING",
                AdvisoryType = "Tower staffing",
                Description = "Weekend tower staffing may delay LAANC approvals. Submit at least 3 hours in advance.",
                EffectiveFrom = missionPlan.PlannedDate.Date,
                EffectiveTo = missionPlan.PlannedDate.Date.AddDays(1).AddTicks(-1),
                Severity = "Low"
            });
        }

        return Task.FromResult<IReadOnlyList<MissionAirspaceAdvisory>>(advisories);
    }
}

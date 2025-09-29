using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Interfaces.External;
using DroneComply.Core.Models;

namespace DroneComply.Core.Services;

public class PreFlightService : IPreFlightService
{
    private readonly IMissionPlanRepository _missionPlanRepository;
    private readonly IAsyncRepository<ComplianceReport> _complianceRepository;
    private readonly IWeatherService _weatherService;
    private readonly IAirspaceAdvisoryService _airspaceAdvisoryService;

    public PreFlightService(
        IMissionPlanRepository missionPlanRepository,
        IAsyncRepository<ComplianceReport> complianceRepository,
        IWeatherService weatherService,
        IAirspaceAdvisoryService airspaceAdvisoryService)
    {
        _missionPlanRepository = missionPlanRepository;
        _complianceRepository = complianceRepository;
        _weatherService = weatherService;
        _airspaceAdvisoryService = airspaceAdvisoryService;
    }

    public async Task<MissionPlan> CreateMissionPlanAsync(MissionPlan plan, CancellationToken cancellationToken = default)
    {
        plan.Id = plan.Id == Guid.Empty ? Guid.NewGuid() : plan.Id;
        plan.Status = MissionStatus.Draft;
        plan.RiskLevel = plan.RiskLevel == 0 ? FlightRiskLevel.Low : plan.RiskLevel;
        return await _missionPlanRepository.AddAsync(plan, cancellationToken);
    }

    public async Task<ComplianceReport> EvaluateComplianceAsync(Guid missionPlanId, CancellationToken cancellationToken = default)
    {
        var missionPlan = await _missionPlanRepository.GetWithDetailsAsync(missionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Mission plan {missionPlanId} was not found.");

        var report = new ComplianceReport
        {
            MissionPlanId = missionPlanId,
            GeneratedOn = DateTime.UtcNow,
            Summary = $"Compliance review for mission '{missionPlan.Name}'"
        };

        var violations = new List<ComplianceViolation>();

        if (missionPlan.PlannedDate < DateTime.UtcNow.AddHours(-1))
        {
            violations.Add(new ComplianceViolation
            {
                Id = Guid.NewGuid(),
                ComplianceReportId = report.Id,
                Code = "MPL-001",
                Title = "Mission plan start time in the past",
                Description = "The mission start time occurs in the past. Update the schedule before flight.",
                Recommendation = "Adjust the planned date/time or create a new mission plan."
            });
        }

        if (missionPlan.Checklist.Any(item => item.IsRequired && !item.IsCompleted))
        {
            violations.Add(new ComplianceViolation
            {
                Id = Guid.NewGuid(),
                ComplianceReportId = report.Id,
                Code = "CHK-001",
                Title = "Required checklist items pending",
                Description = "One or more required checklist items have not been completed.",
                Recommendation = "Complete all required checklist items prior to flight."
            });
        }

        if (missionPlan.RiskLevel == FlightRiskLevel.High)
        {
            violations.Add(new ComplianceViolation
            {
                Id = Guid.NewGuid(),
                ComplianceReportId = report.Id,
                Code = "RISK-001",
                Title = "High risk mission",
                Description = "Mission risk is rated high. Confirm mitigation strategies before approval.",
                Recommendation = "Review mission parameters and update mitigations to reduce risk." 
            });
        }

        if (missionPlan.WeatherBriefing?.Alerts.Any(alert =>
                alert.Severity.Equals("Severe", StringComparison.OrdinalIgnoreCase)) == true)
        {
            violations.Add(new ComplianceViolation
            {
                Id = Guid.NewGuid(),
                ComplianceReportId = report.Id,
                Code = "WX-001",
                Title = "Severe weather advisory",
                Description = "Severe weather alerts are in effect for the mission window.",
                Recommendation = "Delay or relocate the mission until weather conditions improve."
            });
        }

        report.Violations = violations;
        report.Summary = violations.Count == 0
            ? "All compliance checks passed"
            : $"Compliance review identified {violations.Count} issue(s).";

        return await _complianceRepository.AddAsync(report, cancellationToken);
    }

    public async Task<WeatherBriefing> RefreshWeatherBriefingAsync(Guid missionPlanId, CancellationToken cancellationToken = default)
    {
        var missionPlan = await _missionPlanRepository.GetWithDetailsAsync(missionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Mission plan {missionPlanId} was not found.");

        var briefing = await _weatherService.GetWeatherBriefingAsync(missionPlan, cancellationToken);
        briefing.MissionPlanId = missionPlan.Id;

        foreach (var condition in briefing.Conditions)
        {
            condition.WeatherBriefingId = briefing.Id;
        }

        foreach (var alert in briefing.Alerts)
        {
            alert.WeatherBriefingId = briefing.Id;
        }

        missionPlan.WeatherBriefing = briefing;
        await _missionPlanRepository.UpdateAsync(missionPlan, cancellationToken);
        return briefing;
    }

    public async Task<IReadOnlyList<MissionAirspaceAdvisory>> RefreshAirspaceAdvisoriesAsync(Guid missionPlanId, CancellationToken cancellationToken = default)
    {
        var missionPlan = await _missionPlanRepository.GetWithDetailsAsync(missionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Mission plan {missionPlanId} was not found.");

        var advisories = await _airspaceAdvisoryService.GetAirspaceAdvisoriesAsync(missionPlan, cancellationToken);

        missionPlan.AirspaceAdvisories.Clear();
        foreach (var advisory in advisories)
        {
            advisory.MissionPlanId = missionPlan.Id;
            missionPlan.AirspaceAdvisories.Add(advisory);
        }

        await _missionPlanRepository.UpdateAsync(missionPlan, cancellationToken);
        return missionPlan.AirspaceAdvisories;
    }
}

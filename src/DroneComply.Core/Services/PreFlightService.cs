using System;
using System.Collections.Generic;
using System.Linq;
using DroneComply.Core.Enums;
using DroneComply.Core.Exceptions;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Interfaces.External;
using DroneComply.Core.Models;
using DroneComply.Core.Primitives;
using Microsoft.Extensions.Logging;

namespace DroneComply.Core.Services;

public class PreFlightService : IPreFlightService
{
    private readonly IMissionPlanRepository _missionPlanRepository;
    private readonly IAsyncRepository<ComplianceReport> _complianceRepository;
    private readonly IWeatherService _weatherService;
    private readonly IAirspaceAdvisoryService _airspaceAdvisoryService;
    private readonly ILogger<PreFlightService> _logger;

    public PreFlightService(
        IMissionPlanRepository missionPlanRepository,
        IAsyncRepository<ComplianceReport> complianceRepository,
        IWeatherService weatherService,
        IAirspaceAdvisoryService airspaceAdvisoryService,
        ILogger<PreFlightService> logger)
    {
        _missionPlanRepository = missionPlanRepository;
        _complianceRepository = complianceRepository;
        _weatherService = weatherService;
        _airspaceAdvisoryService = airspaceAdvisoryService;
        _logger = logger;
    }

    public async Task<Result<MissionPlan>> CreateMissionPlanAsync(MissionPlan plan, CancellationToken cancellationToken = default)
    {
        try
        {
            plan.Id = plan.Id == Guid.Empty ? Guid.NewGuid() : plan.Id;
            plan.Status = MissionStatus.Draft;
            plan.RiskLevel = plan.RiskLevel == 0 ? FlightRiskLevel.Low : plan.RiskLevel;

            plan.ValidateForScheduling(DateTime.UtcNow);

            var created = await _missionPlanRepository.AddAsync(plan, cancellationToken);
            _logger.LogInformation("Created mission plan {MissionId} for pilot {PilotId}", created.Id, created.PilotId);
            return Result<MissionPlan>.Success(created);
        }
        catch (DomainValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed when creating mission plan {MissionId}", plan.Id);
            return Result<MissionPlan>.ValidationFailure(ex.Message, ex.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating mission plan {MissionId}", plan.Id);
            return Result<MissionPlan>.Failure("Unable to create mission plan. Please try again.");
        }
    }

    public async Task<Result<ComplianceReport>> EvaluateComplianceAsync(Guid missionPlanId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting compliance evaluation for mission {MissionId}", missionPlanId);

            var missionPlan = await _missionPlanRepository.GetWithDetailsAsync(missionPlanId, cancellationToken);
            if (missionPlan is null)
            {
                _logger.LogWarning("Mission plan {MissionId} not found for compliance evaluation", missionPlanId);
                return Result<ComplianceReport>.Failure("Mission plan was not found.");
            }

            var report = BuildComplianceReport(missionPlanId, missionPlan);
            report = await _complianceRepository.AddAsync(report, cancellationToken);

            _logger.LogInformation(
                "Compliance evaluation completed for mission {MissionId}. Found {ViolationCount} violation(s)",
                missionPlanId,
                report.Violations.Count);

            return Result<ComplianceReport>.Success(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during compliance evaluation for mission {MissionId}", missionPlanId);
            return Result<ComplianceReport>.Failure("Compliance evaluation failed. Please try again.");
        }
    }

    public async Task<Result<WeatherBriefing>> RefreshWeatherBriefingAsync(Guid missionPlanId, CancellationToken cancellationToken = default)
    {
        try
        {
            var missionPlan = await _missionPlanRepository.GetWithDetailsAsync(missionPlanId, cancellationToken);
            if (missionPlan is null)
            {
                _logger.LogWarning("Mission plan {MissionId} not found when refreshing weather briefing", missionPlanId);
                return Result<WeatherBriefing>.Failure("Mission plan was not found.");
            }

            _logger.LogInformation("Refreshing weather briefing for mission {MissionId}", missionPlanId);
            var briefing = await _weatherService.GetWeatherBriefingAsync(missionPlan, cancellationToken);
            var persisted = await _missionPlanRepository.ReplaceWeatherBriefingAsync(missionPlan.Id, briefing, cancellationToken);

            missionPlan.WeatherBriefing = persisted;

            return Result<WeatherBriefing>.Success(persisted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh weather briefing for mission {MissionId}", missionPlanId);
            return Result<WeatherBriefing>.Failure("Unable to refresh weather briefing. Please try again.");
        }
    }

    public async Task<Result<IReadOnlyList<MissionAirspaceAdvisory>>> RefreshAirspaceAdvisoriesAsync(Guid missionPlanId, CancellationToken cancellationToken = default)
    {
        try
        {
            var missionPlan = await _missionPlanRepository.GetWithDetailsAsync(missionPlanId, cancellationToken);
            if (missionPlan is null)
            {
                _logger.LogWarning("Mission plan {MissionId} not found when refreshing airspace advisories", missionPlanId);
                return Result<IReadOnlyList<MissionAirspaceAdvisory>>.Failure("Mission plan was not found.");
            }

            _logger.LogInformation("Refreshing airspace advisories for mission {MissionId}", missionPlanId);
            var advisories = await _airspaceAdvisoryService.GetAirspaceAdvisoriesAsync(missionPlan, cancellationToken);

            var persisted = await _missionPlanRepository.ReplaceAirspaceAdvisoriesAsync(missionPlan.Id, advisories, cancellationToken);
            missionPlan.AirspaceAdvisories = persisted.ToList();

            return Result<IReadOnlyList<MissionAirspaceAdvisory>>.Success(persisted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh airspace advisories for mission {MissionId}", missionPlanId);
            return Result<IReadOnlyList<MissionAirspaceAdvisory>>.Failure("Unable to refresh airspace advisories. Please try again.");
        }
    }

    private static ComplianceReport BuildComplianceReport(Guid missionPlanId, MissionPlan missionPlan)
    {
        var report = new ComplianceReport
        {
            MissionPlanId = missionPlanId,
            GeneratedOn = DateTime.UtcNow,
            Summary = $"Compliance review for mission '{missionPlan.Name}'"
        };

        var violations = new List<ComplianceViolation>();

        if (missionPlan.PlannedDate < DateTime.UtcNow.AddHours(-1))
        {
            violations.Add(CreateViolation(
                report.Id,
                "MPL-001",
                "Mission plan start time in the past",
                "The mission start time occurs in the past. Update the schedule before flight.",
                "Adjust the planned date/time or create a new mission plan."));
        }

        if (missionPlan.Checklist.Any(item => item.IsRequired && !item.IsCompleted))
        {
            violations.Add(CreateViolation(
                report.Id,
                "CHK-001",
                "Required checklist items pending",
                "One or more required checklist items have not been completed.",
                "Complete all required checklist items prior to flight."));
        }

        if (missionPlan.RiskLevel == FlightRiskLevel.High)
        {
            violations.Add(CreateViolation(
                report.Id,
                "RISK-001",
                "High risk mission",
                "Mission risk is rated high. Confirm mitigation strategies before approval.",
                "Review mission parameters and update mitigations to reduce risk."));
        }

        if (missionPlan.WeatherBriefing?.Alerts.Any(alert =>
                alert.Severity.Equals("Severe", StringComparison.OrdinalIgnoreCase)) == true)
        {
            violations.Add(CreateViolation(
                report.Id,
                "WX-001",
                "Severe weather advisory",
                "Severe weather alerts are in effect for the mission window.",
                "Delay or relocate the mission until weather conditions improve."));
        }

        report.Violations = violations;
        report.Summary = violations.Count == 0
            ? "All compliance checks passed"
            : $"Compliance review identified {violations.Count} issue(s).";

        return report;
    }

    private static ComplianceViolation CreateViolation(
        Guid complianceReportId,
        string code,
        string title,
        string description,
        string recommendation)
    {
        return new ComplianceViolation
        {
            ComplianceReportId = complianceReportId,
            Code = code,
            Title = title,
            Description = description,
            Recommendation = recommendation
        };
    }
}

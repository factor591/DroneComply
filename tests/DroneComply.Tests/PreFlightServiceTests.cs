using System;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Interfaces.External;
using DroneComply.Core.Models;
using DroneComply.Core.Primitives;
using DroneComply.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace DroneComply.Tests;

public class PreFlightServiceTests
{
    private readonly Mock<IMissionPlanRepository> _missionPlanRepositoryMock = new();
    private readonly Mock<IAsyncRepository<ComplianceReport>> _complianceRepositoryMock = new();
    private readonly Mock<IWeatherService> _weatherServiceMock = new();
    private readonly Mock<IAirspaceAdvisoryService> _airspaceServiceMock = new();
    private readonly Mock<ILogger<PreFlightService>> _loggerMock = new();
    private readonly PreFlightService _service;

    public PreFlightServiceTests()
    {
        _missionPlanRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<MissionPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MissionPlan plan, CancellationToken _) => plan);

        _missionPlanRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<MissionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _complianceRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<ComplianceReport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ComplianceReport report, CancellationToken _) => report);

        _weatherServiceMock
            .Setup(w => w.GetWeatherBriefingAsync(It.IsAny<MissionPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherBriefing());

        _airspaceServiceMock
            .Setup(a => a.GetAirspaceAdvisoriesAsync(It.IsAny<MissionPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<MissionAirspaceAdvisory>());

        _service = new PreFlightService(
            _missionPlanRepositoryMock.Object,
            _complianceRepositoryMock.Object,
            _weatherServiceMock.Object,
            _airspaceServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task EvaluateComplianceAsync_ReturnsFailure_WhenMissionPlanMissing()
    {
        _missionPlanRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MissionPlan?)null);

        var result = await _service.EvaluateComplianceAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Mission plan was not found.", result.Error);
    }

    [Fact]
    public async Task EvaluateComplianceAsync_ReturnsViolations_ForHighRiskMission()
    {
        var missionId = Guid.NewGuid();
        var missionPlan = new MissionPlan
        {
            Id = missionId,
            Name = "High risk mission",
            PilotId = Guid.NewGuid(),
            AircraftId = Guid.NewGuid(),
            PlannedDate = DateTime.UtcNow.AddHours(2),
            RiskLevel = FlightRiskLevel.High,
            WeatherBriefing = new WeatherBriefing
            {
                Alerts =
                {
                    new WeatherAlert { Severity = "Severe" }
                }
            }
        };

        missionPlan.Checklist.Add(new MissionChecklistItem
        {
            IsRequired = true,
            IsCompleted = false
        });

        _missionPlanRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(missionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(missionPlan);

        var result = await _service.EvaluateComplianceAsync(missionId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value!.Violations.Count);
    }

    [Fact]
    public async Task CreateMissionPlanAsync_ReturnsValidationFailure_ForPastMission()
    {
        var missionPlan = new MissionPlan
        {
            Name = "Test",
            PilotId = Guid.NewGuid(),
            AircraftId = Guid.NewGuid(),
            PlannedDate = DateTime.UtcNow.AddHours(-4)
        };

        var result = await _service.CreateMissionPlanAsync(missionPlan);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.ValidationErrors);
        Assert.True(result.ValidationErrors!.ContainsKey(nameof(MissionPlan.PlannedDate)));
    }
}

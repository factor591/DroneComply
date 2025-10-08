using System;
using DroneComply.Core.Exceptions;
using DroneComply.Core.Models;

namespace DroneComply.Tests;

public class MissionPlanTests
{
    [Fact]
    public void ValidateForScheduling_AllowsFutureMission()
    {
        var missionPlan = BuildValidMissionPlan();

        var exception = Record.Exception(() => missionPlan.ValidateForScheduling(DateTime.UtcNow));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateForScheduling_ThrowsWhenPilotMissing()
    {
        var missionPlan = BuildValidMissionPlan();
        missionPlan.PilotId = Guid.Empty;

        var exception = Assert.Throws<DomainValidationException>(() => missionPlan.ValidateForScheduling(DateTime.UtcNow));

        Assert.True(exception.Errors.ContainsKey(nameof(MissionPlan.PilotId)));
    }

    [Fact]
    public void ValidateForScheduling_ThrowsWhenScheduledInPast()
    {
        var missionPlan = BuildValidMissionPlan();
        missionPlan.PlannedDate = DateTime.UtcNow.AddHours(-2);

        var exception = Assert.Throws<DomainValidationException>(() => missionPlan.ValidateForScheduling(DateTime.UtcNow));

        Assert.True(exception.Errors.ContainsKey(nameof(MissionPlan.PlannedDate)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_SetInvalidValue_ThrowsArgumentException(string? invalidName)
    {
        var missionPlan = BuildValidMissionPlan();

        Assert.Throws<ArgumentException>(() => missionPlan.Name = invalidName!);
    }

    private static MissionPlan BuildValidMissionPlan()
    {
        return new MissionPlan
        {
            Name = "Test Mission",
            PilotId = Guid.NewGuid(),
            AircraftId = Guid.NewGuid(),
            PlannedDate = DateTime.UtcNow.AddHours(2),
            LaunchLocation = "Test Launch",
            LandingLocation = "Test Landing"
        };
    }
}

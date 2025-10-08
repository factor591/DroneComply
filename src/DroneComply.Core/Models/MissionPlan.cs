using System;
using System.Collections.Generic;
using System.Linq;
using DroneComply.Core.Enums;
using DroneComply.Core.Exceptions;

namespace DroneComply.Core.Models;

public class MissionPlan : AuditableEntity
{
    private string _name = string.Empty;
    private string _launchLocation = string.Empty;
    private string _landingLocation = string.Empty;
    private string _missionObjective = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name
    {
        get => _name;
        set => _name = ValidateRequired(value, nameof(Name), 200);
    }

    public Guid PilotId { get; set; }

    public Guid AircraftId { get; set; }

    public DateTime PlannedDate { get; set; } = DateTime.UtcNow;

    public string LaunchLocation
    {
        get => _launchLocation;
        set => _launchLocation = ValidateOptional(value, nameof(LaunchLocation), 300);
    }

    public string LandingLocation
    {
        get => _landingLocation;
        set => _landingLocation = ValidateOptional(value, nameof(LandingLocation), 300);
    }

    public MissionStatus Status { get; set; } = MissionStatus.Draft;

    public FlightRiskLevel RiskLevel { get; set; } = FlightRiskLevel.Low;

    public string MissionObjective
    {
        get => _missionObjective;
        set => _missionObjective = ValidateOptional(value, nameof(MissionObjective), 1000);
    }

    public List<MissionChecklistItem> Checklist { get; set; } = new();

    public List<MissionAirspaceAdvisory> AirspaceAdvisories { get; set; } = new();

    public WeatherBriefing? WeatherBriefing { get; set; }

    public void ValidateForScheduling(DateTime utcNow)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (PilotId == Guid.Empty)
        {
            AppendError(errors, nameof(PilotId), "Pilot is required.");
        }

        if (AircraftId == Guid.Empty)
        {
            AppendError(errors, nameof(AircraftId), "Aircraft is required.");
        }

        if (PlannedDate == default)
        {
            AppendError(errors, nameof(PlannedDate), "Planned date is required.");
        }
        else if (PlannedDate < utcNow.AddHours(-1))
        {
            AppendError(errors, nameof(PlannedDate), "Mission cannot be scheduled in the past.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException("Mission plan validation failed.",
                errors.ToDictionary(static pair => pair.Key, static pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase));
        }
    }

    private static string ValidateRequired(string? value, string propertyName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} is required.", propertyName);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"{propertyName} cannot exceed {maxLength} characters.", propertyName);
        }

        return trimmed;
    }

    private static string ValidateOptional(string? value, string propertyName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"{propertyName} cannot exceed {maxLength} characters.", propertyName);
        }

        return trimmed;
    }

    private static void AppendError(IDictionary<string, List<string>> errors, string propertyName, string message)
    {
        if (!errors.TryGetValue(propertyName, out var propertyErrors))
        {
            propertyErrors = new List<string>();
            errors[propertyName] = propertyErrors;
        }

        propertyErrors.Add(message);
    }
}

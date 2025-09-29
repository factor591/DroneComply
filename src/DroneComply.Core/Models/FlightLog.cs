using System;
using System.Collections.Generic;
using DroneComply.Core.Enums;

namespace DroneComply.Core.Models;

public class FlightLog : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PilotId { get; set; }
    public Guid AircraftId { get; set; }
    public DateTime FlightDate { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    public double DistanceCoveredKm { get; set; }
    public FlightPurpose Purpose { get; set; } = FlightPurpose.Commercial;
    public FlightRiskLevel RiskAssessment { get; set; } = FlightRiskLevel.Low;
    public string Location { get; set; } = string.Empty;
    public string AirspaceClassification { get; set; } = string.Empty;
    public string MissionSummary { get; set; } = string.Empty;
    public List<FlightLogEvent> Events { get; set; } = new();
    public List<FlightDocument> Attachments { get; set; } = new();
}

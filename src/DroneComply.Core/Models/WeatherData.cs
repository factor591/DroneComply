namespace DroneComply.Core.Models;

public class MetarData
{
    public string StationId { get; set; } = string.Empty;
    public DateTime ObservationTime { get; set; }
    public string RawText { get; set; } = string.Empty;
    public double? TemperatureCelsius { get; set; }
    public double? DewpointCelsius { get; set; }
    public string WindDirection { get; set; } = string.Empty;
    public int? WindSpeedKnots { get; set; }
    public int? WindGustKnots { get; set; }
    public double? VisibilityStatuteMiles { get; set; }
    public string FlightCategory { get; set; } = string.Empty; // VFR, MVFR, IFR, LIFR
    public List<SkyCondition> SkyConditions { get; set; } = new();
}

public class TafData
{
    public string StationId { get; set; } = string.Empty;
    public DateTime IssueTime { get; set; }
    public DateTime ValidFromTime { get; set; }
    public DateTime ValidToTime { get; set; }
    public string RawText { get; set; } = string.Empty;
    public List<TafForecast> Forecasts { get; set; } = new();
}

public class SkyCondition
{
    public string SkyCover { get; set; } = string.Empty; // SKC, FEW, SCT, BKN, OVC
    public int? CloudBaseFeetAgl { get; set; }
}

public class TafForecast
{
    public DateTime ValidFromTime { get; set; }
    public DateTime ValidToTime { get; set; }
    public string ChangeIndicator { get; set; } = string.Empty; // FM, TEMPO, PROB
    public string WindDirection { get; set; } = string.Empty;
    public int? WindSpeedKnots { get; set; }
    public double? VisibilityStatuteMiles { get; set; }
    public List<SkyCondition> SkyConditions { get; set; } = new();
}

public class StationInfo
{
    public string StationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int ElevationFeet { get; set; }
}

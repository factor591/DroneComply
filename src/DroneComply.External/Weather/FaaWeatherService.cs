using System.Net.Http.Json;
using DroneComply.Core.Interfaces.External;
using DroneComply.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DroneComply.External.Weather;

public class FaaWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FaaWeatherService> _logger;
    private readonly string _weatherEndpoint;

    public FaaWeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<FaaWeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _weatherEndpoint = configuration["ExternalAPIs:FAAWeatherAPI"] ?? "https://aviationweather.gov/api/data";
    }

    public async Task<WeatherBriefing> GetWeatherBriefingAsync(MissionPlan missionPlan, CancellationToken cancellationToken = default)
    {
        WeatherBriefing? briefing = null;

        try
        {
            // Placeholder call for future integration - returns null today because FAA API does not expose a public JSON schema.
            using var request = new HttpRequestMessage(HttpMethod.Get, _weatherEndpoint);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                briefing = await response.Content.ReadFromJsonAsync<WeatherBriefing>(cancellationToken: cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falling back to synthetic weather briefing for mission {MissionPlan}", missionPlan.Id);
        }

        if (briefing is null)
        {
            briefing = BuildSyntheticBriefing(missionPlan);
        }

        return briefing;
    }

    private static WeatherBriefing BuildSyntheticBriefing(MissionPlan missionPlan)
    {
        var briefing = new WeatherBriefing
        {
            MissionPlanId = missionPlan.Id,
            Source = "FAA Aviation Weather Center (simulated)",
            Summary = $"VFR conditions expected near {missionPlan.LaunchLocation} on {missionPlan.PlannedDate:MMMM d, yyyy HH:mm} UTC.",
            RetrievedAt = DateTime.UtcNow
        };

        var condition = new WeatherCondition
        {
            WeatherBriefingId = briefing.Id,
            Location = missionPlan.LaunchLocation,
            ObservationTime = DateTime.UtcNow,
            TemperatureCelsius = 22,
            WindSpeedKnots = 8,
            WindGustKnots = 12,
            WindDirectionDegrees = 210,
            VisibilityMiles = 10,
            CeilingFeet = 4500,
            WeatherPhenomena = "CLR"
        };

        briefing.Conditions.Add(condition);

        // Provide an advisory when missions occur at night.
        if (missionPlan.PlannedDate.TimeOfDay > TimeSpan.FromHours(20) || missionPlan.PlannedDate.TimeOfDay < TimeSpan.FromHours(6))
        {
            briefing.Alerts.Add(new WeatherAlert
            {
                WeatherBriefingId = briefing.Id,
                Title = "Night operations",
                Severity = "Moderate",
                Description = "Verify night waiver, lighting, and visual observer requirements for night operations.",
                Effective = DateTime.UtcNow,
                Expires = missionPlan.PlannedDate.AddHours(4)
            });
        }

        return briefing;
    }
}

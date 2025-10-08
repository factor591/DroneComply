using System.Net.Http.Json;
using System.Text.Json;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DroneComply.External.Services;

public class AviationWeatherService : IAviationWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AviationWeatherService> _logger;
    private readonly string _baseUrl;

    public AviationWeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<AviationWeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["ExternalAPIs:FAAWeatherAPI"] ?? "https://aviationweather.gov/data/api/";
    }

    public async Task<MetarData?> GetMetarAsync(string stationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_baseUrl}metar?ids={stationId}&format=json";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch METAR for {StationId}: {StatusCode}", stationId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonArray = JsonDocument.Parse(content).RootElement;

            if (jsonArray.GetArrayLength() == 0)
                return null;

            var metar = jsonArray[0];
            return new MetarData
            {
                StationId = metar.GetProperty("icaoId").GetString() ?? stationId,
                ObservationTime = metar.GetProperty("obsTime").GetDateTime(),
                RawText = metar.GetProperty("rawOb").GetString() ?? string.Empty,
                TemperatureCelsius = GetNullableDouble(metar, "temp"),
                DewpointCelsius = GetNullableDouble(metar, "dewp"),
                WindDirection = metar.TryGetProperty("wdir", out var wdir) ? wdir.GetInt32().ToString() : "VRB",
                WindSpeedKnots = GetNullableInt(metar, "wspd"),
                WindGustKnots = GetNullableInt(metar, "wgst"),
                VisibilityStatuteMiles = GetNullableDouble(metar, "visib"),
                FlightCategory = metar.TryGetProperty("fltcat", out var cat) ? cat.GetString() ?? "UNKNOWN" : "UNKNOWN"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching METAR for {StationId}", stationId);
            return null;
        }
    }

    public async Task<TafData?> GetTafAsync(string stationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_baseUrl}taf?ids={stationId}&format=json";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch TAF for {StationId}: {StatusCode}", stationId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonArray = JsonDocument.Parse(content).RootElement;

            if (jsonArray.GetArrayLength() == 0)
                return null;

            var taf = jsonArray[0];
            return new TafData
            {
                StationId = taf.GetProperty("icaoId").GetString() ?? stationId,
                IssueTime = taf.GetProperty("issueTime").GetDateTime(),
                ValidFromTime = taf.GetProperty("validTimeFrom").GetDateTime(),
                ValidToTime = taf.GetProperty("validTimeTo").GetDateTime(),
                RawText = taf.GetProperty("rawTAF").GetString() ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching TAF for {StationId}", stationId);
            return null;
        }
    }

    private static double? GetNullableDouble(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number)
            return prop.GetDouble();
        return null;
    }

    private static int? GetNullableInt(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number)
            return prop.GetInt32();
        return null;
    }
}

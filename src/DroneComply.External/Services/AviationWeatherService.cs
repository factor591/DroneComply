using System.Collections.Generic;
using System.Globalization;
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

        var configuredBase = configuration["ExternalAPIs:FAAWeatherAPI"] ?? "https://aviationweather.gov/api/data/";
        _baseUrl = EnsureTrailingSlash(configuredBase);
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
            using var document = JsonDocument.Parse(content);
            var jsonArray = document.RootElement;

            if (jsonArray.ValueKind != JsonValueKind.Array || jsonArray.GetArrayLength() == 0)
            {
                return null;
            }

            var metar = jsonArray[0];
            return new MetarData
            {
                StationId = GetString(metar, "icaoId") ?? stationId,
                ObservationTime = GetDateTime(metar, "obsTime") ?? DateTime.UtcNow,
                RawText = GetString(metar, "rawOb") ?? string.Empty,
                TemperatureCelsius = GetNullableDouble(metar, "temp"),
                DewpointCelsius = GetNullableDouble(metar, "dewp"),
                WindDirection = GetWindDirection(metar),
                WindSpeedKnots = GetNullableInt(metar, "wspd"),
                WindGustKnots = GetNullableInt(metar, "wgst"),
                VisibilityStatuteMiles = GetNullableDouble(metar, "visib"),
                FlightCategory = GetString(metar, "fltCat") ?? "UNKNOWN",
                SkyConditions = ParseSkyConditions(metar, "clouds")
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
            using var document = JsonDocument.Parse(content);
            var jsonArray = document.RootElement;

            if (jsonArray.ValueKind != JsonValueKind.Array || jsonArray.GetArrayLength() == 0)
            {
                return null;
            }

            var taf = jsonArray[0];
            return new TafData
            {
                StationId = GetString(taf, "icaoId") ?? stationId,
                IssueTime = GetDateTime(taf, "issueTime") ?? DateTime.UtcNow,
                ValidFromTime = GetDateTime(taf, "validTimeFrom") ?? DateTime.UtcNow,
                ValidToTime = GetDateTime(taf, "validTimeTo") ?? DateTime.UtcNow,
                RawText = GetString(taf, "rawTAF") ?? string.Empty,
                Forecasts = ParseForecasts(taf)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching TAF for {StationId}", stationId);
            return null;
        }
    }

    private static string EnsureTrailingSlash(string value) =>
        value.EndsWith("/", StringComparison.Ordinal) ? value : $"{value}/";

    private static string? GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString(),
            JsonValueKind.Number => property.ToString(),
            _ => null
        };
    }

    private static DateTime? GetDateTime(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number => DateTimeOffset.FromUnixTimeSeconds(property.GetInt64()).UtcDateTime,
            JsonValueKind.String => ParseIsoDateTime(property.GetString()),
            _ => null
        };
    }

    private static DateTime? ParseIsoDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto)
            ? dto.UtcDateTime
            : null;
    }

    private static double? GetNullableDouble(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number)
        {
            return property.GetDouble();
        }

        if (property.ValueKind == JsonValueKind.String)
        {
            return TryParseDouble(property.GetString());
        }

        return null;
    }

    private static int? GetNullableInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number)
        {
            return property.GetInt32();
        }

        if (property.ValueKind == JsonValueKind.String &&
            int.TryParse(property.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        return null;
    }

    private static double? TryParseDouble(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var sanitized = raw.Trim();
        sanitized = sanitized.TrimEnd('+');
        sanitized = sanitized.Replace("SM", string.Empty, StringComparison.OrdinalIgnoreCase);

        if (sanitized.StartsWith("P", StringComparison.OrdinalIgnoreCase))
        {
            sanitized = sanitized[1..];
        }

        return double.TryParse(sanitized, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    private static string GetWindDirection(JsonElement element)
    {
        if (element.TryGetProperty("wdir", out var property))
        {
            if (property.ValueKind == JsonValueKind.String)
            {
                var value = property.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetInt32().ToString(CultureInfo.InvariantCulture);
            }
        }

        return "VRB";
    }

    private static List<SkyCondition> ParseSkyConditions(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array)
        {
            return new List<SkyCondition>();
        }

        var result = new List<SkyCondition>();
        foreach (var item in property.EnumerateArray())
        {
            result.Add(new SkyCondition
            {
                SkyCover = GetString(item, "cover") ?? string.Empty,
                CloudBaseFeetAgl = GetNullableInt(item, "base")
            });
        }

        return result;
    }

    private static List<TafForecast> ParseForecasts(JsonElement taf)
    {
        if (!taf.TryGetProperty("fcsts", out var forecastsElement) || forecastsElement.ValueKind != JsonValueKind.Array)
        {
            return new List<TafForecast>();
        }

        var forecasts = new List<TafForecast>();
        foreach (var forecast in forecastsElement.EnumerateArray())
        {
            forecasts.Add(new TafForecast
            {
                ValidFromTime = GetDateTime(forecast, "timeFrom") ?? DateTime.UtcNow,
                ValidToTime = GetDateTime(forecast, "timeTo") ?? DateTime.UtcNow,
                ChangeIndicator = GetString(forecast, "fcstChange") ?? string.Empty,
                WindDirection = GetWindDirection(forecast),
                WindSpeedKnots = GetNullableInt(forecast, "wspd"),
                VisibilityStatuteMiles = GetNullableDouble(forecast, "visib"),
                SkyConditions = ParseSkyConditions(forecast, "clouds")
            });
        }

        return forecasts;
    }
}

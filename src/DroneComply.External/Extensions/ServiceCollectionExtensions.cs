using DroneComply.Core.Interfaces;
using DroneComply.Core.Interfaces.External;
using DroneComply.External.Airspace;
using DroneComply.External.Services;
using DroneComply.External.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DroneComply.External.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IWeatherService, FaaWeatherService>();
        services.AddHttpClient<IAviationWeatherService, AviationWeatherService>();
        services.AddScoped<IAirspaceAdvisoryService, AloftAirspaceService>();
        return services;
    }
}

using DroneComply.Core.Interfaces.External;
using DroneComply.External.Airspace;
using DroneComply.External.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DroneComply.External.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IWeatherService, FaaWeatherService>();
        services.AddScoped<IAirspaceAdvisoryService, AloftAirspaceService>();
        return services;
    }
}

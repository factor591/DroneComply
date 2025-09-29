using DroneComply.Core.Interfaces;
using DroneComply.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DroneComply.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IPreFlightService, PreFlightService>();
        services.AddScoped<IFlightLogService, FlightLogService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        return services;
    }
}

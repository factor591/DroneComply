using DroneComply.Core.Interfaces;
using DroneComply.Data.Context;
using DroneComply.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DroneComply.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DroneComplyDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
        services.AddScoped<IMissionPlanRepository, MissionPlanRepository>();
        services.AddScoped<IFlightLogRepository, FlightLogRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

        return services;
    }
}

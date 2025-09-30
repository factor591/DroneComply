using System;
using DroneComply.Core.Interfaces;
using DroneComply.Data.Context;
using DroneComply.Data.Repositories;
using DroneComply.Data.Seeding;
using Microsoft.Data.Sqlite;
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

        var passwordEnvVariable = configuration["Secrets:Database:PasswordEnvironmentVariable"];
        if (!string.IsNullOrWhiteSpace(passwordEnvVariable))
        {
            var password = Environment.GetEnvironmentVariable(passwordEnvVariable);
            if (!string.IsNullOrWhiteSpace(password))
            {
                var builder = new SqliteConnectionStringBuilder(connectionString)
                {
                    Password = password
                };

                connectionString = builder.ToString();
            }
        }

        services.AddDbContext<DroneComplyDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
        services.AddScoped<IMissionPlanRepository, MissionPlanRepository>();
        services.AddScoped<IFlightLogRepository, FlightLogRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IDbSeeder, DbSeeder>();

        return services;
    }
}




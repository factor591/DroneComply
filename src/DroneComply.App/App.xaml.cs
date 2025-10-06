using System;
using System.IO;
using DroneComply.App.Services;
using DroneComply.App.ViewModels;
using DroneComply.App.Views;
using DroneComply.Core.Extensions;
using DroneComply.Data.Context;
using DroneComply.Data.Extensions;
using DroneComply.Data.Seeding;
using DroneComply.External.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;
using Serilog.Settings.Configuration;

namespace DroneComply.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Host = CreateHostBuilder().Build();
    }

    public IHost Host { get; }

    public static T GetService<T>() where T : class
    {
        if (((Application.Current as App)?.Host.Services.GetService(typeof(T))) is T service)
        {
            return service;
        }

        throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        // Initialize database
        await InitializeDatabaseAsync();

        var window = Host.Services.GetRequiredService<MainWindow>();
        window.Activate();
    }

    private async System.Threading.Tasks.Task InitializeDatabaseAsync()
    {
        try
        {
            using var scope = Host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DroneComplyDbContext>();

            // Apply migrations
            await dbContext.Database.MigrateAsync();

            // Seed data
            var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = Host.Services.GetRequiredService<ILogger<App>>();
            logger.LogError(ex, "An error occurred while initializing the database");
        }
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.SetBasePath(AppContext.BaseDirectory);
                configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configurationBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder => builder.AddSerilog());

                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<ShellViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<PreFlightViewModel>();
                services.AddTransient<FlightLogViewModel>();
                services.AddTransient<MaintenanceViewModel>();
                services.AddTransient<WaiverViewModel>();
                services.AddTransient<SettingsViewModel>();

                services.AddSingleton<ShellPage>();
                services.AddSingleton<MainWindow>();

                services.AddCoreServices();
                services.AddDataLayer(context.Configuration);
                services.AddExternalIntegrations(context.Configuration);
            })
            .UseSerilog((context, services, configuration) =>
            {
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Logs"));
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Debug()
                    .WriteTo.File(
                        Path.Combine(AppContext.BaseDirectory, "Logs", "dronecomply-.log"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
            });
    }
}




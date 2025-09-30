using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Windows.ApplicationModel.DataTransfer;

namespace DroneComply.App.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private string _databaseConnection = string.Empty;

    [ObservableProperty]
    private string _faaWeatherEndpoint = string.Empty;

    [ObservableProperty]
    private string _aloftApiKey = string.Empty;

    [ObservableProperty]
    private string _googleMapsApiKey = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public SettingsViewModel(IConfiguration configuration)
    {
        _configuration = configuration;
        LoadSettings();
        CopyConnectionStringCommand = new RelayCommand(CopyConnectionString);
    }

    public IRelayCommand CopyConnectionStringCommand { get; }

    private void LoadSettings()
    {
        DatabaseConnection = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        FaaWeatherEndpoint = _configuration["ExternalAPIs:FAAWeatherAPI"] ?? string.Empty;
        AloftApiKey = DescribeSecret("Secrets:ApiKeys:Aloft");
        GoogleMapsApiKey = DescribeSecret("Secrets:ApiKeys:GoogleMaps");

        StatusMessage = "Secrets are sourced from environment variables. Update appsettings.*.json to change bindings.";
    }

    private string DescribeSecret(string secretPath)
    {
        var envVariable = _configuration[secretPath];
        if (string.IsNullOrWhiteSpace(envVariable))
        {
            return "Not configured";
        }

        var isSet = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(envVariable));
        return isSet
            ? $"Environment variable '{envVariable}' is configured."
            : $"Environment variable '{envVariable}' is not set.";
    }

    private void CopyConnectionString()
    {
        try
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(DatabaseConnection);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
            StatusMessage = "Connection string copied to clipboard.";
        }
        catch
        {
            StatusMessage = "Unable to copy the connection string. Please set clipboard permissions.";
        }
    }
}

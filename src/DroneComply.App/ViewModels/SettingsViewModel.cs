using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Windows.ApplicationModel.DataTransfer;

namespace DroneComply.App.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IConfiguration _configuration;
    private string _aloftApiKeyEnvVar = string.Empty;
    private string _googleMapsApiKeyEnvVar = string.Empty;

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

    [ObservableProperty]
    private bool _isEditingAloft = false;

    [ObservableProperty]
    private bool _isEditingGoogleMaps = false;

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

        _aloftApiKeyEnvVar = _configuration["Secrets:ApiKeys:Aloft"] ?? string.Empty;
        _googleMapsApiKeyEnvVar = _configuration["Secrets:ApiKeys:GoogleMaps"] ?? string.Empty;

        AloftApiKey = GetApiKeyValue(_aloftApiKeyEnvVar);
        GoogleMapsApiKey = GetApiKeyValue(_googleMapsApiKeyEnvVar);

        StatusMessage = "API keys are stored in environment variables.";
    }

    private string GetApiKeyValue(string envVarName)
    {
        if (string.IsNullOrWhiteSpace(envVarName))
        {
            return "Not configured";
        }

        var value = Environment.GetEnvironmentVariable(envVarName);
        return string.IsNullOrWhiteSpace(value) ? "Not set" : MaskApiKey(value);
    }

    private string MaskApiKey(string apiKey)
    {
        if (apiKey.Length <= 8)
            return "****";

        return apiKey.Substring(0, 4) + new string('*', apiKey.Length - 8) + apiKey.Substring(apiKey.Length - 4);
    }

    [RelayCommand]
    private void EditAloftApiKey()
    {
        if (IsEditingAloft)
        {
            // Cancel edit
            AloftApiKey = GetApiKeyValue(_aloftApiKeyEnvVar);
            IsEditingAloft = false;
        }
        else
        {
            // Start editing - show actual value
            var actualValue = Environment.GetEnvironmentVariable(_aloftApiKeyEnvVar) ?? string.Empty;
            AloftApiKey = actualValue;
            IsEditingAloft = true;
        }
    }

    [RelayCommand]
    private void SaveAloftApiKey()
    {
        try
        {
            Environment.SetEnvironmentVariable(_aloftApiKeyEnvVar, AloftApiKey, EnvironmentVariableTarget.User);
            AloftApiKey = GetApiKeyValue(_aloftApiKeyEnvVar);
            IsEditingAloft = false;
            StatusMessage = "Aloft API key saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save API key: {ex.Message}";
        }
    }

    [RelayCommand]
    private void EditGoogleMapsApiKey()
    {
        if (IsEditingGoogleMaps)
        {
            // Cancel edit
            GoogleMapsApiKey = GetApiKeyValue(_googleMapsApiKeyEnvVar);
            IsEditingGoogleMaps = false;
        }
        else
        {
            // Start editing - show actual value
            var actualValue = Environment.GetEnvironmentVariable(_googleMapsApiKeyEnvVar) ?? string.Empty;
            GoogleMapsApiKey = actualValue;
            IsEditingGoogleMaps = true;
        }
    }

    [RelayCommand]
    private void SaveGoogleMapsApiKey()
    {
        try
        {
            Environment.SetEnvironmentVariable(_googleMapsApiKeyEnvVar, GoogleMapsApiKey, EnvironmentVariableTarget.User);
            GoogleMapsApiKey = GetApiKeyValue(_googleMapsApiKeyEnvVar);
            IsEditingGoogleMaps = false;
            StatusMessage = "Google Maps API key saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save API key: {ex.Message}";
        }
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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

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
        AloftApiKey = _configuration["ExternalAPIs:AloftAPI"] ?? string.Empty;
        GoogleMapsApiKey = _configuration["ExternalAPIs:GoogleMapsAPI"] ?? string.Empty;

        StatusMessage = "Edit appsettings.json to update values.";
    }

    private void CopyConnectionString()
    {
        try
        {
            Windows.ApplicationModel.DataTransfer.Clipboard.SetText(DatabaseConnection);
            StatusMessage = "Connection string copied to clipboard.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}

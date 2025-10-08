using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.App.ViewModels;

public partial class WeatherViewModel : ObservableRecipient
{
    private readonly IAviationWeatherService _weatherService;

    [ObservableProperty]
    private string _stationId = "KBOS"; // Default to Boston

    [ObservableProperty]
    private MetarData? _currentMetar;

    [ObservableProperty]
    private TafData? _currentTaf;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public WeatherViewModel(IAviationWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await LoadWeatherAsync();
    }

    [RelayCommand]
    private async Task LoadWeatherAsync()
    {
        if (string.IsNullOrWhiteSpace(StationId))
        {
            StatusMessage = "Please enter a station ID (e.g., KBOS, KJFK)";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = $"Loading weather for {StationId.ToUpper()}...";

            var metarTask = _weatherService.GetMetarAsync(StationId.ToUpper());
            var tafTask = _weatherService.GetTafAsync(StationId.ToUpper());

            await Task.WhenAll(metarTask, tafTask);

            CurrentMetar = await metarTask;
            CurrentTaf = await tafTask;

            if (CurrentMetar == null && CurrentTaf == null)
            {
                StatusMessage = $"No weather data found for {StationId.ToUpper()}. Verify station ID.";
            }
            else
            {
                StatusMessage = $"Weather data loaded for {StationId.ToUpper()}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load weather: {ex.Message}";
            CurrentMetar = null;
            CurrentTaf = null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshWeatherAsync()
    {
        await LoadWeatherAsync();
    }
}

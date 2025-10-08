using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.ViewModels;

public partial class FlightLogViewModel : ObservableRecipient
{
    private readonly IFlightLogRepository _flightLogRepository;
    private readonly IFlightLogService _flightLogService;

    [ObservableProperty]
    private ObservableCollection<FlightLog> _flightLogs = new();

    [ObservableProperty]
    private FlightLog? _selectedFlightLog;

    [ObservableProperty]
    private string _pilotFilter = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public FlightLogViewModel(IFlightLogRepository flightLogRepository, IFlightLogService flightLogService)
    {
        _flightLogRepository = flightLogRepository;
        _flightLogService = flightLogService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => SelectedFlightLog is not null && !IsBusy);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedFlightLog is not null && !IsBusy);
    }

    partial void OnSelectedFlightLogChanged(FlightLog? value)
    {
        StatusMessage = string.Empty;
        SaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            FlightLogs.Clear();

            IReadOnlyList<FlightLog> logs;
            if (Guid.TryParse(PilotFilter, out var pilotId))
            {
                logs = await _flightLogRepository.GetForPilotAsync(pilotId);
            }
            else
            {
                logs = await _flightLogRepository.ListAsync();
            }

            foreach (var log in logs)
            {
                FlightLogs.Add(log);
            }

            if (FlightLogs.Count == 0)
            {
                StatusMessage = "No flight logs found.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load flight logs: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedFlightLog is null)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await _flightLogService.UpdateAsync(SelectedFlightLog);
            StatusMessage = "Flight log saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedFlightLog is null || App.MainWindow?.Content == null)
        {
            return;
        }

        var dialog = new ContentDialog
        {
            Title = "Delete Flight Log",
            Content = $"Are you sure you want to delete this flight log from {SelectedFlightLog.FlightDate:d}?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary)
            return;

        try
        {
            IsBusy = true;
            await _flightLogService.DeleteAsync(SelectedFlightLog.Id);
            FlightLogs.Remove(SelectedFlightLog);
            SelectedFlightLog = null;
            StatusMessage = "Flight log removed.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

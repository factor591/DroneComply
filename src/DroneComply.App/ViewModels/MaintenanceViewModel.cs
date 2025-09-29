using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.App.ViewModels;

public partial class MaintenanceViewModel : ObservableRecipient
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    private readonly IMaintenanceService _maintenanceService;

    [ObservableProperty]
    private ObservableCollection<MaintenanceRecord> _maintenanceRecords = new();

    [ObservableProperty]
    private string _aircraftFilter = string.Empty;

    [ObservableProperty]
    private MaintenanceRecord? _selectedRecord;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public MaintenanceViewModel(IMaintenanceRepository maintenanceRepository, IMaintenanceService maintenanceService)
    {
        _maintenanceRepository = maintenanceRepository;
        _maintenanceService = maintenanceService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        CompleteTaskCommand = new AsyncRelayCommand<MaintenanceTask?>(CompleteTaskAsync);
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<MaintenanceTask?> CompleteTaskCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            MaintenanceRecords.Clear();

            IReadOnlyList<MaintenanceRecord> records;
            if (Guid.TryParse(AircraftFilter, out var aircraftId))
            {
                records = await _maintenanceRepository.GetOutstandingAsync(aircraftId);
            }
            else
            {
                records = await _maintenanceRepository.ListAsync();
            }

            foreach (var record in records)
            {
                MaintenanceRecords.Add(record);
            }

            if (MaintenanceRecords.Count == 0)
            {
                StatusMessage = "No maintenance items due.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load maintenance: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CompleteTaskAsync(MaintenanceTask? task)
    {
        if (task is null)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await _maintenanceService.CompleteTaskAsync(task.Id);
            task.IsCompleted = true;
            task.CompletedOn = DateTime.UtcNow;
            StatusMessage = "Task completed.";
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

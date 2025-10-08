using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.App.ViewModels;

public partial class MaintenanceViewModel : ObservableRecipient
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    private readonly IMaintenanceService _maintenanceService;
    private readonly IAircraftRepository _aircraftRepository;

    [ObservableProperty]
    private ObservableCollection<Aircraft> _aircraft = new();

    [ObservableProperty]
    private Aircraft? _selectedAircraft;

    [ObservableProperty]
    private ObservableCollection<MaintenanceRecord> _maintenanceRecords = new();

    [ObservableProperty]
    private MaintenanceRecord? _selectedRecord;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public MaintenanceViewModel(
        IMaintenanceRepository maintenanceRepository,
        IMaintenanceService maintenanceService,
        IAircraftRepository aircraftRepository)
    {
        _maintenanceRepository = maintenanceRepository;
        _maintenanceService = maintenanceService;
        _aircraftRepository = aircraftRepository;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        CompleteTaskCommand = new AsyncRelayCommand<MaintenanceTask?>(CompleteTaskAsync);
    }

    partial void OnSelectedAircraftChanged(Aircraft? value)
    {
        if (value == null)
        {
            MaintenanceRecords.Clear();
            SelectedRecord = null;
            return;
        }

        _ = LoadMaintenanceForAircraftAsync();
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<MaintenanceTask?> CompleteTaskCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;

            var aircraftList = await _aircraftRepository.ListAsync();
            var previouslySelectedId = SelectedAircraft?.Id;
            Aircraft = new ObservableCollection<Aircraft>(aircraftList);

            if (Aircraft.Count > 0)
            {
                SelectedAircraft = Aircraft.FirstOrDefault(a => a.Id == previouslySelectedId) ?? Aircraft.First();
            }
            else
            {
                SelectedAircraft = null;
            }

            StatusMessage = aircraftList.Count == 0
                ? "No aircraft found."
                : $"{aircraftList.Count} aircraft loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load aircraft: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMaintenanceForAircraftAsync()
    {
        if (SelectedAircraft == null) return;

        try
        {
            IsBusy = true;
            var previouslySelectedRecordId = SelectedRecord?.Id;
            MaintenanceRecords.Clear();

            var records = await _maintenanceRepository.GetOutstandingAsync(SelectedAircraft.Id);

            foreach (var record in records)
            {
                MaintenanceRecords.Add(record);
            }

            SelectedRecord = MaintenanceRecords.FirstOrDefault(r => r.Id == previouslySelectedRecordId) ?? MaintenanceRecords.FirstOrDefault();

            if (MaintenanceRecords.Count == 0)
            {
                StatusMessage = $"No maintenance records for {SelectedAircraft.Name}.";
            }
            else
            {
                StatusMessage = $"Loaded {MaintenanceRecords.Count} maintenance record(s).";
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

    [RelayCommand]
    private async Task AddMaintenanceRecordAsync()
    {
        if (SelectedAircraft == null) return;

        try
        {
            var newRecord = new MaintenanceRecord
            {
                Id = Guid.NewGuid(),
                AircraftId = SelectedAircraft.Id,
                Type = MaintenanceType.Scheduled,
                Status = MaintenanceStatus.Scheduled,
                ScheduledDate = DateTime.Now,
                Description = "New Maintenance Record",
                Notes = string.Empty
            };

            await _maintenanceRepository.AddAsync(newRecord);
            MaintenanceRecords.Add(newRecord);
            SelectedRecord = newRecord;
            StatusMessage = "New maintenance record created.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to add maintenance record: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveMaintenanceRecordAsync(MaintenanceRecord? record)
    {
        if (record == null) return;

        try
        {
            await _maintenanceRepository.UpdateAsync(record);
            StatusMessage = "Maintenance record saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save maintenance record: {ex.Message}";
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
            OnPropertyChanged(nameof(SelectedRecord));
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

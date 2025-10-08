using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.ViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly IMissionPlanRepository _missionPlanRepository;
    private readonly IFlightLogService _flightLogService;
    private readonly IPilotRepository _pilotRepository;
    private readonly IAircraftRepository _aircraftRepository;

    [ObservableProperty]
    private ObservableCollection<MissionPlan> _upcomingMissions = new();

    [ObservableProperty]
    private ObservableCollection<Pilot> _availablePilots = new();

    [ObservableProperty]
    private ObservableCollection<Aircraft> _availableAircraft = new();

    [ObservableProperty]
    private int _pendingApprovalCount;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private MissionPlan? _selectedMission;

    public DashboardViewModel(
        IMissionPlanRepository missionPlanRepository,
        IFlightLogService flightLogService,
        IPilotRepository pilotRepository,
        IAircraftRepository aircraftRepository)
    {
        _missionPlanRepository = missionPlanRepository;
        _flightLogService = flightLogService;
        _pilotRepository = pilotRepository;
        _aircraftRepository = aircraftRepository;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            var missions = await _missionPlanRepository.ListAsync();
            UpcomingMissions = new ObservableCollection<MissionPlan>(missions);
            PendingApprovalCount = missions.Count(m => m.Status == MissionStatus.PendingApproval);

            var pilots = await _pilotRepository.GetActivePilotsAsync();
            AvailablePilots = new ObservableCollection<Pilot>(pilots);

            var aircraft = await _aircraftRepository.GetActiveAircraftAsync();
            AvailableAircraft = new ObservableCollection<Aircraft>(aircraft);

            StatusMessage = missions.Count == 0
                ? "No missions scheduled. Create a mission to get started."
                : "";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load missions: {ex.Message}";
            UpcomingMissions.Clear();
            PendingApprovalCount = 0;
        }
    }

    [RelayCommand]
    private async Task AddMissionAsync()
    {
        try
        {
            // Use first available pilot and aircraft if available
            var defaultPilot = AvailablePilots.FirstOrDefault();
            var defaultAircraft = AvailableAircraft.FirstOrDefault();

            var newMission = new MissionPlan
            {
                Id = Guid.NewGuid(),
                Name = "New Mission",
                MissionObjective = "Enter mission objective",
                PlannedDate = DateTime.Now.AddDays(1),
                Status = MissionStatus.Draft,
                RiskLevel = FlightRiskLevel.Low,
                LaunchLocation = "TBD",
                LandingLocation = "TBD",
                PilotId = defaultPilot?.Id ?? Guid.Empty,
                AircraftId = defaultAircraft?.Id ?? Guid.Empty
            };

            await _missionPlanRepository.AddAsync(newMission);
            UpcomingMissions.Add(newMission);
            SelectedMission = newMission;
            StatusMessage = "";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to add mission: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteMissionAsync(MissionPlan? mission)
    {
        if (mission == null || App.MainWindow?.Content == null) return;

        var dialog = new ContentDialog
        {
            Title = "Delete Mission",
            Content = $"Are you sure you want to delete the mission '{mission.Name}'?",
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
            await _missionPlanRepository.DeleteAsync(mission.Id);
            UpcomingMissions.Remove(mission);

            if (SelectedMission == mission)
                SelectedMission = null;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to delete mission: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RenameMissionAsync(MissionPlan? mission)
    {
        if (mission == null) return;

        try
        {
            await _missionPlanRepository.UpdateAsync(mission);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to rename mission: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task UpdateMissionAsync(MissionPlan? mission)
    {
        if (mission == null)
        {
            StatusMessage = "No mission selected.";
            return;
        }

        try
        {
            StatusMessage = "Saving...";
            await _missionPlanRepository.UpdateAsync(mission);

            // Refresh the mission in the list to show updated values
            var index = UpcomingMissions.IndexOf(mission);
            if (index >= 0)
            {
                UpcomingMissions.RemoveAt(index);
                UpcomingMissions.Insert(index, mission);
                SelectedMission = mission;
            }

            StatusMessage = $"Mission '{mission.Name}' saved successfully at {DateTime.Now:HH:mm:ss}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to update mission: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CompleteMissionAsync(MissionPlan? mission)
    {
        if (mission == null) return;

        try
        {
            // Create a flight log from the mission
            var flightLog = new FlightLog
            {
                Id = Guid.NewGuid(),
                PilotId = mission.PilotId,
                AircraftId = mission.AircraftId,
                FlightDate = mission.PlannedDate,
                Purpose = FlightPurpose.Commercial,
                RiskAssessment = mission.RiskLevel,
                Location = mission.LaunchLocation,
                MissionSummary = $"{mission.Name}: {mission.MissionObjective}",
                Duration = TimeSpan.Zero, // Can be updated later
                DistanceCoveredKm = 0, // Can be updated later
                AirspaceClassification = string.Empty
            };

            // Save the flight log
            await _flightLogService.CreateAsync(flightLog);

            // Update mission status to completed
            mission.Status = MissionStatus.Completed;
            await _missionPlanRepository.UpdateAsync(mission);

            // Remove from upcoming missions list
            UpcomingMissions.Remove(mission);

            if (SelectedMission == mission)
                SelectedMission = null;

            StatusMessage = "Mission completed and added to flight logs.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to complete mission: {ex.Message}";
        }
    }
}

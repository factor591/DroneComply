using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.App.ViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly IMissionPlanRepository _missionPlanRepository;

    [ObservableProperty]
    private ObservableCollection<MissionPlan> _upcomingMissions = new();

    [ObservableProperty]
    private int _pendingApprovalCount;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private MissionPlan? _selectedMission;

    public DashboardViewModel(IMissionPlanRepository missionPlanRepository)
    {
        _missionPlanRepository = missionPlanRepository;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            var missions = await _missionPlanRepository.ListAsync();
            UpcomingMissions = new ObservableCollection<MissionPlan>(missions);
            PendingApprovalCount = missions.Count(m => m.Status == MissionStatus.PendingApproval);
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
                PilotId = Guid.Empty,
                AircraftId = Guid.Empty
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
        if (mission == null) return;

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
}

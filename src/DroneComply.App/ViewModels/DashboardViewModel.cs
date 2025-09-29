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

    public DashboardViewModel(IMissionPlanRepository missionPlanRepository)
    {
        _missionPlanRepository = missionPlanRepository;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public IAsyncRelayCommand RefreshCommand { get; }

    public async Task LoadAsync()
    {
        try
        {
            var missions = await _missionPlanRepository.GetUpcomingAsync(5);
            UpcomingMissions = new ObservableCollection<MissionPlan>(missions);
            PendingApprovalCount = missions.Count(m => m.Status == MissionStatus.PendingApproval);
            StatusMessage = missions.Count == 0
                ? "No missions scheduled. Create a mission to get started."
                : "";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load dashboard data: {ex.Message}";
            UpcomingMissions.Clear();
            PendingApprovalCount = 0;
        }
    }
}

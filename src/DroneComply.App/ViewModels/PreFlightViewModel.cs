using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.App.ViewModels;

public partial class PreFlightViewModel : ObservableRecipient
{
    private readonly IMissionPlanRepository _missionPlanRepository;
    private readonly IPreFlightService _preFlightService;

    [ObservableProperty]
    private ObservableCollection<MissionPlan> _missionPlans = new();

    [ObservableProperty]
    private MissionPlan? _selectedMission;

    [ObservableProperty]
    private ComplianceReport? _latestReport;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public PreFlightViewModel(IMissionPlanRepository missionPlanRepository, IPreFlightService preFlightService)
    {
        _missionPlanRepository = missionPlanRepository;
        _preFlightService = preFlightService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        EvaluateComplianceCommand = new AsyncRelayCommand(EvaluateComplianceAsync, () => SelectedMission is not null && !IsBusy);
        RefreshWeatherCommand = new AsyncRelayCommand(RefreshWeatherAsync, () => SelectedMission is not null && !IsBusy);
        RefreshAirspaceCommand = new AsyncRelayCommand(RefreshAirspaceAsync, () => SelectedMission is not null && !IsBusy);
    }

    partial void OnSelectedMissionChanged(MissionPlan? value)
    {
        LatestReport = null;
        StatusMessage = string.Empty;
        EvaluateComplianceCommand.NotifyCanExecuteChanged();
        RefreshWeatherCommand.NotifyCanExecuteChanged();
        RefreshAirspaceCommand.NotifyCanExecuteChanged();
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand EvaluateComplianceCommand { get; }
    public IAsyncRelayCommand RefreshWeatherCommand { get; }
    public IAsyncRelayCommand RefreshAirspaceCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            MissionPlans.Clear();
            var plans = await _missionPlanRepository.GetUpcomingAsync(25);
            foreach (var plan in plans)
            {
                MissionPlans.Add(plan);
            }

            if (MissionPlans.Count > 0)
            {
                SelectedMission = MissionPlans.First();
            }
            else
            {
                SelectedMission = null;
                StatusMessage = "Create a mission plan to begin pre-flight checks.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load mission plans: {ex.Message}";
        }
    }

    private async Task EvaluateComplianceAsync()
    {
        if (SelectedMission is null)
        {
            return;
        }

        await RunMissionOperationAsync(async () =>
        {
            LatestReport = await _preFlightService.EvaluateComplianceAsync(SelectedMission.Id);
            StatusMessage = LatestReport.IsCompliant
                ? "Mission is compliant with current checks."
                : $"Mission has {LatestReport.Violations.Count} compliance issue(s).";
        });
    }

    private async Task RefreshWeatherAsync()
    {
        if (SelectedMission is null)
        {
            return;
        }

        await RunMissionOperationAsync(async () =>
        {
            var briefing = await _preFlightService.RefreshWeatherBriefingAsync(SelectedMission.Id);
            SelectedMission.WeatherBriefing = briefing;
            StatusMessage = "Weather briefing updated.";
        });
    }

    private async Task RefreshAirspaceAsync()
    {
        if (SelectedMission is null)
        {
            return;
        }

        await RunMissionOperationAsync(async () =>
        {
            var advisories = await _preFlightService.RefreshAirspaceAdvisoriesAsync(SelectedMission.Id);
            SelectedMission.AirspaceAdvisories.Clear();
            foreach (var advisory in advisories)
            {
                SelectedMission.AirspaceAdvisories.Add(advisory);
            }

            StatusMessage = "Airspace advisories refreshed.";
        });
    }

    private async Task RunMissionOperationAsync(Func<Task> operation)
    {
        try
        {
            IsBusy = true;
            EvaluateComplianceCommand.NotifyCanExecuteChanged();
            RefreshWeatherCommand.NotifyCanExecuteChanged();
            RefreshAirspaceCommand.NotifyCanExecuteChanged();

            await operation();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            EvaluateComplianceCommand.NotifyCanExecuteChanged();
            RefreshWeatherCommand.NotifyCanExecuteChanged();
            RefreshAirspaceCommand.NotifyCanExecuteChanged();
        }
    }
}

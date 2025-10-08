using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using DroneComply.Core.Primitives;

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
        catch
        {
            StatusMessage = "Failed to load mission plans. Please try again.";
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
            var result = await _preFlightService.EvaluateComplianceAsync(SelectedMission.Id);
            if (result.IsFailure || result.Value is null)
            {
                LatestReport = null;
                StatusMessage = BuildErrorMessage(result, "Compliance evaluation failed.");
                return;
            }

            LatestReport = result.Value;
            StatusMessage = result.Value.IsCompliant
                ? "Mission is compliant with current checks."
                : $"Mission has {result.Value.Violations.Count} compliance issue(s).";
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
            var result = await _preFlightService.RefreshWeatherBriefingAsync(SelectedMission.Id);
            if (result.IsFailure || result.Value is null)
            {
                StatusMessage = BuildErrorMessage(result, "Weather briefing update failed.");
                return;
            }

            SelectedMission.WeatherBriefing = result.Value;
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
            var result = await _preFlightService.RefreshAirspaceAdvisoriesAsync(SelectedMission.Id);
            if (result.IsFailure || result.Value is null)
            {
                StatusMessage = BuildErrorMessage(result, "Airspace advisories update failed.");
                return;
            }

            SelectedMission.AirspaceAdvisories.Clear();
            foreach (var advisory in result.Value)
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
        catch
        {
            StatusMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            IsBusy = false;
            EvaluateComplianceCommand.NotifyCanExecuteChanged();
            RefreshWeatherCommand.NotifyCanExecuteChanged();
            RefreshAirspaceCommand.NotifyCanExecuteChanged();
        }
    }

    private static string BuildErrorMessage<T>(Result<T> result, string fallbackMessage)
    {
        if (!string.IsNullOrWhiteSpace(result.Error))
        {
            return result.Error;
        }

        if (result.ValidationErrors?.Count > 0)
        {
            var first = result.ValidationErrors.First();
            var combined = string.Join(", ", first.Value);
            return string.IsNullOrWhiteSpace(combined)
                ? fallbackMessage
                : $"{first.Key}: {combined}";
        }

        return fallbackMessage;
    }
}

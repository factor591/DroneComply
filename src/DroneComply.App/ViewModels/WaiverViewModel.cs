using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;

namespace DroneComply.App.ViewModels;

public partial class WaiverViewModel : ObservableRecipient
{
    private readonly IAsyncRepository<Waiver> _waiverRepository;

    [ObservableProperty]
    private ObservableCollection<Waiver> _waivers = new();

    [ObservableProperty]
    private Waiver? _selectedWaiver;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public WaiverViewModel(IAsyncRepository<Waiver> waiverRepository)
    {
        _waiverRepository = waiverRepository;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => SelectedWaiver is not null && !IsBusy);
    }

    partial void OnSelectedWaiverChanged(Waiver? value)
    {
        if (value is not null && value.Status == 0)
        {
            value.Status = WaiverStatus.Draft;
        }

        SaveCommand.NotifyCanExecuteChanged();
        StatusMessage = string.Empty;
    }

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            Waivers.Clear();
            var waivers = await _waiverRepository.ListAsync();
            foreach (var waiver in waivers.OrderByDescending(w => w.SubmittedOn))
            {
                Waivers.Add(waiver);
            }

            if (Waivers.Count == 0)
            {
                StatusMessage = "No waivers on file.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load waivers: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedWaiver is null)
        {
            return;
        }

        try
        {
            IsBusy = true;
            await _waiverRepository.UpdateAsync(SelectedWaiver);
            StatusMessage = "Waiver updated.";
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

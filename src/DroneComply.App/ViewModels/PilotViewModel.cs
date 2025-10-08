using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.ViewModels;

public partial class PilotViewModel : ObservableRecipient
{
    private readonly IPilotRepository _pilotRepository;

    [ObservableProperty]
    private ObservableCollection<Pilot> _pilots = new();

    [ObservableProperty]
    private Pilot? _selectedPilot;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public PilotViewModel(IPilotRepository pilotRepository)
    {
        _pilotRepository = pilotRepository;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            var pilots = await _pilotRepository.ListAsync();
            Pilots = new ObservableCollection<Pilot>(pilots);
            StatusMessage = pilots.Count == 0
                ? "No pilots found. Add a pilot to get started."
                : $"{pilots.Count} pilot(s) loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load pilots: {ex.Message}";
            Pilots.Clear();
        }
    }

    [RelayCommand]
    private async Task AddPilotAsync()
    {
        try
        {
            var newPilot = new Pilot
            {
                Id = Guid.NewGuid(),
                FirstName = "New",
                LastName = "Pilot",
                Email = string.Empty,
                PhoneNumber = string.Empty,
                CertificateNumber = string.Empty,
                CertificationDate = DateTime.Now,
                IsActive = true
            };

            await _pilotRepository.AddAsync(newPilot);
            Pilots.Add(newPilot);
            SelectedPilot = newPilot;
            IsEditing = true;
            StatusMessage = "New pilot created. Please enter details.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to add pilot: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SavePilotAsync(Pilot? pilot)
    {
        if (pilot == null) return;

        try
        {
            await _pilotRepository.UpdateAsync(pilot);

            // Refresh the pilot in the list
            var index = Pilots.IndexOf(pilot);
            if (index >= 0)
            {
                Pilots.RemoveAt(index);
                Pilots.Insert(index, pilot);
                SelectedPilot = pilot;
            }

            IsEditing = false;
            StatusMessage = $"Pilot {pilot.FullName} saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save pilot: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeletePilotAsync(Pilot? pilot)
    {
        if (pilot == null || App.MainWindow?.Content == null) return;

        var dialog = new ContentDialog
        {
            Title = "Delete Pilot",
            Content = $"Are you sure you want to delete '{pilot.FullName}'?",
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
            await _pilotRepository.DeleteAsync(pilot.Id);
            Pilots.Remove(pilot);

            if (SelectedPilot == pilot)
                SelectedPilot = null;

            StatusMessage = $"Pilot {pilot.FullName} deleted.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to delete pilot: {ex.Message}";
        }
    }

    [RelayCommand]
    private void EditPilot(Pilot? pilot)
    {
        if (pilot == null) return;
        SelectedPilot = pilot;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task CancelEditAsync()
    {
        if (SelectedPilot == null)
        {
            IsEditing = false;
            return;
        }

        try
        {
            // Reload the pilot from the database to discard changes
            var freshPilot = await _pilotRepository.GetByIdAsync(SelectedPilot.Id);
            if (freshPilot != null)
            {
                var index = Pilots.IndexOf(SelectedPilot);
                if (index >= 0)
                {
                    Pilots[index] = freshPilot;
                    SelectedPilot = freshPilot;
                }
            }

            IsEditing = false;
            StatusMessage = "Edit cancelled.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to cancel: {ex.Message}";
            IsEditing = false;
        }
    }

    [RelayCommand]
    private async Task SearchPilotsAsync()
    {
        try
        {
            var allPilots = await _pilotRepository.ListAsync();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Pilots = new ObservableCollection<Pilot>(allPilots);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filtered = allPilots.Where(p =>
                    p.FirstName.ToLower().Contains(searchLower) ||
                    p.LastName.ToLower().Contains(searchLower) ||
                    p.Email.ToLower().Contains(searchLower) ||
                    p.CertificateNumber.ToLower().Contains(searchLower)
                ).ToList();

                Pilots = new ObservableCollection<Pilot>(filtered);
            }

            StatusMessage = $"Found {Pilots.Count} pilot(s).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Search failed: {ex.Message}";
        }
    }
}

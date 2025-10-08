using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneComply.Core.Enums;
using DroneComply.Core.Interfaces;
using DroneComply.Core.Models;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.ViewModels;

public partial class AircraftViewModel : ObservableRecipient
{
    private readonly IAircraftRepository _aircraftRepository;

    public IReadOnlyList<AircraftType> AircraftTypes { get; } = Enum.GetValues<AircraftType>();

    public IReadOnlyList<AircraftStatus> AircraftStatuses { get; } = Enum.GetValues<AircraftStatus>();

    [ObservableProperty]
    private ObservableCollection<Aircraft> _aircraft = new();

    [ObservableProperty]
    private Aircraft? _selectedAircraft;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public AircraftViewModel(IAircraftRepository aircraftRepository)
    {
        _aircraftRepository = aircraftRepository;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            var previouslySelectedId = SelectedAircraft?.Id;
            var aircraft = await _aircraftRepository.ListAsync();
            Aircraft = new ObservableCollection<Aircraft>(aircraft);
            SelectedAircraft = Aircraft.FirstOrDefault(a => a.Id == previouslySelectedId) ?? Aircraft.FirstOrDefault();
            StatusMessage = aircraft.Count == 0
                ? "No aircraft found. Add aircraft to get started."
                : $"{aircraft.Count} aircraft loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load aircraft: {ex.Message}";
            Aircraft.Clear();
        }
    }

    [RelayCommand]
    private async Task AddAircraftAsync()
    {
        try
        {
            var newAircraft = new Aircraft
            {
                Id = Guid.NewGuid(),
                Name = "New Aircraft",
                Manufacturer = string.Empty,
                Model = string.Empty,
                SerialNumber = string.Empty,
                Type = AircraftType.Multirotor,
                Status = AircraftStatus.Active,
                CommissionedOn = DateTime.Now,
                MaxTakeoffWeightKg = 0,
                MaxFlightTimeMinutes = 0
            };

            await _aircraftRepository.AddAsync(newAircraft);
            Aircraft.Add(newAircraft);
            SelectedAircraft = newAircraft;
            IsEditing = true;
            StatusMessage = "New aircraft created. Please enter details.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to add aircraft: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SaveAircraftAsync(Aircraft? aircraft)
    {
        if (aircraft == null) return;

        try
        {
            await _aircraftRepository.UpdateAsync(aircraft);
            IsEditing = false;
            StatusMessage = $"Aircraft {aircraft.Name} saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save aircraft: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteAircraftAsync(Aircraft? aircraft)
    {
        if (aircraft == null || App.MainWindow?.Content == null) return;

        var dialog = new ContentDialog
        {
            Title = "Delete Aircraft",
            Content = $"Are you sure you want to delete '{aircraft.Name}'?",
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
            await _aircraftRepository.DeleteAsync(aircraft.Id);
            Aircraft.Remove(aircraft);

            if (SelectedAircraft == aircraft)
                SelectedAircraft = Aircraft.FirstOrDefault();

            StatusMessage = $"Aircraft {aircraft.Name} deleted.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to delete aircraft: {ex.Message}";
        }
    }

    [RelayCommand]
    private void EditAircraft(Aircraft? aircraft)
    {
        if (aircraft == null) return;
        SelectedAircraft = aircraft;
        IsEditing = true;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        StatusMessage = "Edit cancelled.";
    }

    [RelayCommand]
    private async Task AddEquipmentAsync(Aircraft? aircraft)
    {
        if (aircraft == null) return;

        var newEquipment = new AircraftEquipment
        {
            Id = Guid.NewGuid(),
            Name = "New Equipment",
            Manufacturer = string.Empty,
            Version = string.Empty,
            InstalledOn = DateTime.Now
        };

        aircraft.Equipment.Add(newEquipment);

        try
        {
            await _aircraftRepository.UpdateAsync(aircraft);
            StatusMessage = "Equipment added.";
        }
        catch (Exception ex)
        {
            aircraft.Equipment.Remove(newEquipment);
            StatusMessage = $"Failed to add equipment: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RemoveEquipmentAsync(AircraftEquipment? equipment)
    {
        if (SelectedAircraft == null || equipment == null)
        {
            return;
        }

        var index = SelectedAircraft.Equipment.IndexOf(equipment);
        if (index < 0)
        {
            return;
        }

        try
        {
            SelectedAircraft.Equipment.RemoveAt(index);
            await _aircraftRepository.UpdateAsync(SelectedAircraft);
            StatusMessage = "Equipment removed.";
        }
        catch (Exception ex)
        {
            SelectedAircraft.Equipment.Insert(index, equipment);
            StatusMessage = $"Failed to remove equipment: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SearchAircraftAsync()
    {
        try
        {
            var allAircraft = await _aircraftRepository.ListAsync();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Aircraft = new ObservableCollection<Aircraft>(allAircraft);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filtered = allAircraft.Where(a =>
                    a.Name.ToLower().Contains(searchLower) ||
                    a.Manufacturer.ToLower().Contains(searchLower) ||
                    a.Model.ToLower().Contains(searchLower) ||
                    a.SerialNumber.ToLower().Contains(searchLower)
                ).ToList();

                Aircraft = new ObservableCollection<Aircraft>(filtered);
            }

            StatusMessage = $"Found {Aircraft.Count} aircraft.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Search failed: {ex.Message}";
        }
    }
}


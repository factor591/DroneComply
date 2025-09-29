using CommunityToolkit.Mvvm.ComponentModel;
using DroneComply.App.Views;

namespace DroneComply.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    public ShellViewModel()
    {
        PrimaryItems = new List<ShellNavigationItem>
        {
            new("Dashboard", "\uE80F", typeof(DashboardPage)),
            new("Pre-Flight", "\uE709", typeof(PreFlightPage)),
            new("Flight Logs", "\uE81E", typeof(FlightLogPage)),
            new("Maintenance", "\uE90F", typeof(MaintenancePage)),
            new("Waivers", "\uE8B8", typeof(WaiverPage))
        };

        SecondaryItems = new List<ShellNavigationItem>
        {
            new("Settings", "\uE713", typeof(SettingsPage))
        };
    }

    public IReadOnlyList<ShellNavigationItem> PrimaryItems { get; }
    public IReadOnlyList<ShellNavigationItem> SecondaryItems { get; }
}

public record ShellNavigationItem(string Label, string Glyph, Type PageType);

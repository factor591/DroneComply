using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
        : this(App.GetService<SettingsViewModel>())
    {
    }

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    public SettingsViewModel ViewModel { get; }
}

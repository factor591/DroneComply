using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DroneComply.App.Views;

public sealed partial class AircraftPage : Page
{
    public AircraftViewModel ViewModel { get; }

    public AircraftPage()
    {
        ViewModel = App.GetService<AircraftViewModel>();
        InitializeComponent();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}


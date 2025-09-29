using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Views;

public sealed partial class FlightLogPage : Page
{
    public FlightLogPage()
        : this(App.GetService<FlightLogViewModel>())
    {
    }

    public FlightLogPage(FlightLogViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
        Loaded += OnLoaded;
    }

    public FlightLogViewModel ViewModel { get; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}

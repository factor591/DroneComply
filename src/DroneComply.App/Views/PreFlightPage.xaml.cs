using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Views;

public sealed partial class PreFlightPage : Page
{
    public PreFlightPage()
        : this(App.GetService<PreFlightViewModel>())
    {
    }

    public PreFlightPage(PreFlightViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
        Loaded += OnLoaded;
    }

    public PreFlightViewModel ViewModel { get; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}

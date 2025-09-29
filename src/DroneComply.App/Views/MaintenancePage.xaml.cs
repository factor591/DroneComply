using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Views;

public sealed partial class MaintenancePage : Page
{
    public MaintenancePage()
        : this(App.GetService<MaintenanceViewModel>())
    {
    }

    public MaintenancePage(MaintenanceViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
        Loaded += OnLoaded;
    }

    public MaintenanceViewModel ViewModel { get; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}

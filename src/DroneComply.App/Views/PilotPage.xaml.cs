using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DroneComply.App.Views;

public sealed partial class PilotPage : Page
{
    public PilotViewModel ViewModel { get; }

    public PilotPage()
    {
        ViewModel = App.GetService<PilotViewModel>();
        InitializeComponent();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}


using System;
using DroneComply.App.ViewModels;
using DroneComply.Core.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Views;

public sealed partial class WaiverPage : Page
{
    public WaiverPage()
        : this(App.GetService<WaiverViewModel>())
    {
    }

    public WaiverPage(WaiverViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
        StatusCombo.ItemsSource = Enum.GetValues(typeof(WaiverStatus));
        Loaded += OnLoaded;
    }

    public WaiverViewModel ViewModel { get; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}

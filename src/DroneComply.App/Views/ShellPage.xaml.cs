using System.Linq;
using DroneComply.App.Services;
using DroneComply.App.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Views;

public sealed partial class ShellPage : Page
{
    private readonly INavigationService _navigationService;

    public ShellPage(ShellViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        ViewModel = viewModel;
        _navigationService = navigationService;
        DataContext = ViewModel;
    }

    public ShellViewModel ViewModel { get; }

    public Frame NavigationFrame => ContentFrame;

    private void OnNavigationViewLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _navigationService.Initialize(ContentFrame);
        BuildNavigationMenu();

        if (ShellNavigationView.MenuItems.FirstOrDefault() is NavigationViewItem firstItem && firstItem.Tag is ShellNavigationItem navItem)
        {
            ShellNavigationView.SelectedItem = firstItem;
            NavigateTo(navItem);
        }
    }

    private void BuildNavigationMenu()
    {
        ShellNavigationView.MenuItems.Clear();
        foreach (var navItem in ViewModel.PrimaryItems)
        {
            ShellNavigationView.MenuItems.Add(CreateNavigationViewItem(navItem));
        }

        ShellNavigationView.FooterMenuItems.Clear();
        foreach (var navItem in ViewModel.SecondaryItems)
        {
            ShellNavigationView.FooterMenuItems.Add(CreateNavigationViewItem(navItem));
        }
    }

    private static NavigationViewItem CreateNavigationViewItem(ShellNavigationItem navItem)
    {
        return new NavigationViewItem
        {
            Content = navItem.Label,
            Tag = navItem,
            Icon = new FontIcon { Glyph = navItem.Glyph }
        };
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            var settings = ViewModel.SecondaryItems.FirstOrDefault(item => item.PageType == typeof(SettingsPage));
            if (settings is not null)
            {
                NavigateTo(settings);
            }

            return;
        }

        if (args.InvokedItemContainer?.Tag is ShellNavigationItem navItem)
        {
            NavigateTo(navItem);
        }
    }

    private void NavigateTo(ShellNavigationItem navItem)
    {
        if (_navigationService.Navigate(navItem.PageType))
        {
            ShellNavigationView.Header = navItem.Label;
        }
    }
}

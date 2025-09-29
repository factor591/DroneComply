using DroneComply.App.Views;
using Microsoft.UI.Xaml;

namespace DroneComply.App;

public sealed partial class MainWindow : Window
{
    public MainWindow(ShellPage shellPage)
    {
        InitializeComponent();
        Title = "DroneComply";
        Content = shellPage;
        shellPage.NavigationFrame.Navigated += (_, args) => Title = args.SourcePageType.Name + " - DroneComply";
    }
}

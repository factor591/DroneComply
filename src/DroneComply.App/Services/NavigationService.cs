using Microsoft.UI.Xaml.Controls;

namespace DroneComply.App.Services;

public interface INavigationService
{
    void Initialize(Frame frame);
    bool Navigate(Type pageType, object? parameter = null);
    bool CanGoBack { get; }
    void GoBack();
}

public class NavigationService : INavigationService
{
    private Frame? _frame;

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    public void Initialize(Frame frame)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
    }

    public bool Navigate(Type pageType, object? parameter = null)
    {
        if (_frame is null)
        {
            throw new InvalidOperationException("Navigation service has not been initialized.");
        }

        if (_frame.CurrentSourcePageType == pageType)
        {
            return false;
        }

        return _frame.Navigate(pageType, parameter);
    }

    public void GoBack()
    {
        if (CanGoBack)
        {
            _frame?.GoBack();
        }
    }
}

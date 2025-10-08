using System;
using Microsoft.UI.Xaml.Data;

namespace DroneComply.App.Converters;

public sealed class DateTimeToTimeSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.TimeOfDay;
        }

        return TimeSpan.Zero;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            return DateTime.Today.Add(timeSpan);
        }

        return DateTime.Now;
    }
}

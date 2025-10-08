using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace DroneComply.App.Converters;

public sealed class DateTimeToDateTimeOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            DateTime dateTime => new DateTimeOffset(dateTime),
            DateTimeOffset dateTimeOffset => dateTimeOffset,
            _ => DependencyProperty.UnsetValue
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            DateTimeOffset dateTimeOffset => dateTimeOffset.DateTime,
            DateTime dateTime => dateTime,
            _ => DependencyProperty.UnsetValue
        };
    }
}


using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace DroneComply.App.Converters;

public sealed class FormattingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (parameter is string format && !string.IsNullOrWhiteSpace(format))
        {
            if (format.Contains("{0", StringComparison.Ordinal))
            {
                return string.Format(CultureInfo.CurrentCulture, format, value);
            }

            if (value is IFormattable formattable)
            {
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace timActionCenter.Presentation;

public sealed class ActiveButtonStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string resourceKey = value is true ? "AccentButtonStyle" : "DefaultButtonStyle";
        return Application.Current.TryFindResource(resourceKey) ?? DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

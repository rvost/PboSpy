using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace PboExplorer.Modules.ConfigExplorer.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public BoolToVisibilityConverter()
    {
        True = Visibility.Visible;
        False = Visibility.Collapsed;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Binding.DoNothing;

        if (value is bool == false)
            return Binding.DoNothing;

        bool input = (bool)value;

        if (input == true)
            return True;

        return False;
    }

    public Visibility True { get; set; }

    public Visibility False { get; set; }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

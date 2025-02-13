using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace io_simulation_wpf.Converters
{
    public class LedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOn)
            {
                return isOn ? Brushes.LimeGreen : Brushes.DarkGray; 
            }
            return Brushes.DarkGray; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

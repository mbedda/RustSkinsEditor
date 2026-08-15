using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RustSkinsEditor.Converters
{
    public class ItemSupportedToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromRgb(229, 57, 53));

        static ItemSupportedToBrushConverter()
        {
            RedBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSupported && !isSupported)
            {
                return RedBrush;
            }

            if (parameter is string defaultColor && !string.IsNullOrEmpty(defaultColor))
            {
                var brush = new BrushConverter().ConvertFromString(defaultColor) as Brush;
                if (brush != null)
                    return brush;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

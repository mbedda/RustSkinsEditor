using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RustSkinsEditor.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private bool inverse = false;
        public bool Inverse
        {
            get { return inverse; }
            set { inverse = value; }
        }

        //Set to true if you just want to hide the control
        //else set to false if you want to collapse the control
        private bool isHidden;
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; }
        }

        private object GetVisibility(object value)
        {
            if (value is not bool) return DependencyProperty.UnsetValue;

            bool booleanInput = Inverse ? !(bool)value : (bool)value;

            return booleanInput ? Visibility.Visible : IsHidden ? Visibility.Hidden : Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RustSkinsEditor.Converters
{
    public class UriFromShortnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            else
            {
                return new Uri(value.ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

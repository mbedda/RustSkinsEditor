using RustSkinsEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RustSkinsEditor.Converters
{
    public class IsMarketSkinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && (ulong)value != 0)
            {
				MainViewModel vm = ((MainWindow)Application.Current.MainWindow).viewModel;

                return vm.RustItems.DLCsData.ProhibitedSkins.Contains((ulong)value) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

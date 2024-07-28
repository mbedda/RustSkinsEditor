using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RustSkinsEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainWindow GetMainWindow()
        {
            return ((MainWindow)System.Windows.Application.Current.MainWindow);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred.. please send below details to dev in a support ticket to resolve:\n\n" + e.Exception.ToString(), "Exception");
        }
    }
}

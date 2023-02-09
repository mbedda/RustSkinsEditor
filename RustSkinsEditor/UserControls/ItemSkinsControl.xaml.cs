using RustSkinsEditor.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustSkinsEditor.UserControls
{
    /// <summary>
    /// Interaction logic for ItemSkinsControl.xaml
    /// </summary>
    public partial class ItemSkinsControl : UserControl
    {
        public ItemSkinsControl()
        {
            InitializeComponent();

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void OpenLink_Click(object sender, RoutedEventArgs e)
        {
            SteamSkinDetails skinDetails = (SteamSkinDetails)((MenuItem)sender).CommandParameter;

            Process.Start(new ProcessStartInfo(skinDetails.WorkshopUrl.AbsoluteUri) { UseShellExecute = true });
        }

        private void CopyId_Click(object sender, RoutedEventArgs e)
        {
            SteamSkinDetails skinDetails = (SteamSkinDetails)((MenuItem)sender).CommandParameter;

            Clipboard.SetText(skinDetails.Code.ToString());
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            SteamSkinDetails skinDetails = (SteamSkinDetails)((MenuItem)sender).CommandParameter;

            ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.SetFullscreen(skinDetails.PreviewUrl);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected skin?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SteamSkinDetails skinDetails = (SteamSkinDetails)((MenuItem)sender).CommandParameter;

                ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.Delete(skinDetails.shortname, skinDetails.Code);
                List<SteamSkinDetails> skinDetailsList = (List<SteamSkinDetails>)DataContext;
                skinDetailsList.Remove(skinDetails);
                DataContext = null;
                DataContext = skinDetailsList;
            }
        }

        private void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (SkinsListbox.SelectedItems.Count > 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected skins?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        List<SteamSkinDetails> skinDetailsList = (List<SteamSkinDetails>)DataContext;
                        foreach (var item in SkinsListbox.SelectedItems.Cast<SteamSkinDetails>())
                        {
                            ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.Delete(item.shortname, item.Code);
                            skinDetailsList.Remove(item);
                        }

                        DataContext = null;
                        DataContext = skinDetailsList;
                    }
                }
            }
        }

        private void SkinItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                SteamSkinDetails skinDetails = (sender as StackPanel).DataContext as SteamSkinDetails;

                if (skinDetails != null)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.SetFullscreen(skinDetails.PreviewUrl);
                }
            }
        }
    }
}

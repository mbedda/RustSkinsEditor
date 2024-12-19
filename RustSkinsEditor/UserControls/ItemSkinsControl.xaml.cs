using RustSkinsEditor.Models;
using RustSkinsEditor.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RustSkinsEditor.UserControls
{
    /// <summary>
    /// Interaction logic for ItemSkinsControl.xaml
    /// </summary>
    public partial class ItemSkinsControl : UserControl
    {
        MainViewModel mainViewModel;
        public ItemSkinsControl()
        {
            InitializeComponent();

        }

        private void SkinsListbox_Loaded(object sender, RoutedEventArgs e)
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow) != null)
                mainViewModel = ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel;
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

                //mainViewModel.Delete(skinDetails.shortname, skinDetails.Code);
                //List<SteamSkinDetails> skinDetailsList = (List<SteamSkinDetails>)DataContext;
                mainViewModel.SelectedItemSkins.Remove(skinDetails);
                //DataContext = null;
                //DataContext = skinDetailsList;
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
                        for (int i = SkinsListbox.SelectedItems.Count-1; i >= 0; i--)
                        {
                            SteamSkinDetails skinDetails = (SteamSkinDetails)SkinsListbox.SelectedItems[i];

                            //mainViewModel.Delete(skinDetails.shortname, skinDetails.Code);
                            mainViewModel.SelectedItemSkins.Remove(skinDetails);
                        }

                        //DataContext = null;
                        //DataContext = skinDetailsList;
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
                    mainViewModel.SetFullscreen(skinDetails.PreviewUrl);
                }
            }
        }

        private void SkinsListbox_Drop(object sender, DragEventArgs e)
        {
            //((MainWindow)System.Windows.Application.Current.MainWindow).UpdateItemSkinsControlIfComboSelected();
        }
    }
}

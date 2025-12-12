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
            BaseSkin baseSkin = (BaseSkin)((MenuItem)sender).CommandParameter;

            if (baseSkin.WorkshopUrl == null) return;

            Process.Start(new ProcessStartInfo(baseSkin.WorkshopUrl.AbsoluteUri) { UseShellExecute = true });
        }

        private void CopyId_Click(object sender, RoutedEventArgs e)
        {
            BaseSkin baseSkin = (BaseSkin)((MenuItem)sender).CommandParameter;

            Clipboard.SetDataObject(baseSkin.WorkshopId.ToString());
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            BaseSkin baseSkin = (BaseSkin)((MenuItem)sender).CommandParameter;

            ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.SetFullscreen(baseSkin.PreviewUrl);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected skins?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                for (int i = SkinsListbox.SelectedItems.Count - 1; i >= 0; i--)
                {
                    BaseSkin baseSkin = (BaseSkin)SkinsListbox.SelectedItems[i];

                    mainViewModel.SelectedItem.Skins.Remove(baseSkin);
                }
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
                            BaseSkin baseSkin = (BaseSkin)SkinsListbox.SelectedItems[i];

                            mainViewModel.SelectedItem.Skins.Remove(baseSkin);
                        }
                    }
                }
            }
        }

        private void SkinItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                BaseSkin baseSkin = (sender as StackPanel).DataContext as BaseSkin;

                if (baseSkin != null)
                {
                    mainViewModel.SetFullscreen(baseSkin.PreviewUrl);
                }
            }
        }

        private void SkinsListbox_Drop(object sender, DragEventArgs e)
        {
            //((MainWindow)System.Windows.Application.Current.MainWindow).UpdateItemSkinsControlIfComboSelected();
        }
    }
}

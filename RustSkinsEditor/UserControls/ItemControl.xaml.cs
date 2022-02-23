using RustSkinsEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    /// Interaction logic for ItemControl.xaml
    /// </summary>
    public partial class ItemControl : UserControl
    {
        public ItemControl()
        {
            InitializeComponent();
        }
        private void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (ItemListbox.SelectedItems.Count > 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected item with all it's skins?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        List<Skin> skinItemList = (List<Skin>)DataContext;
                        foreach (var item in ItemListbox.SelectedItems.Cast<Skin>())
                        {
                            skinItemList.Remove(item);
                        }

                        ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.UpdateActivity();

                        DataContext = null;
                        DataContext = skinItemList;
                    }
                }
            }
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Skin skinitem = (sender as StackPanel).DataContext as Skin;

                if (skinitem != null)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).comboboxItems.SelectedItem = skinitem;
                    filtertxt.Text = "";
                    ICollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ItemListbox.ItemsSource);

                    if (itemsViewOriginal != null)
                        itemsViewOriginal.Filter = null;

                    ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.ExitItemsModal();
                }
            }
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ICollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ItemListbox.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(filtertxt.Text)) return true;
                else
                {
                    if (((Skin)o).Name.ToLower().Contains(filtertxt.Text.Trim().ToLower())) return true;
                    else return false;
                }
            });
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            filtertxt.Text = "";
            ICollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ItemListbox.ItemsSource);

            if (itemsViewOriginal != null)
                itemsViewOriginal.Filter = null;

            ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.ExitItemsModal();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected item with all it's skins?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Skin skinItem = (Skin)((MenuItem)sender).CommandParameter;

                //((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.Delete(skinItem);
                List<Skin> skinItems = (List<Skin>)DataContext;
                skinItems.Remove(skinItem);
                ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.UpdateActivity();
                DataContext = null;
                DataContext = skinItems;
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewItemShortnameTB.Text.Trim() == "")
            {
                MessageBox.Show("Shortname is required.");
                return;
            }

            if (((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.AddItem(NewItemShortnameTB.Text.Trim()))
            {
                MessageBox.Show("Item added successfully!");
            }

            NewItemNameTB.Text = "";
            NewItemShortnameTB.Text = "";
            List<Skin> skinItems = (List<Skin>)DataContext;
            DataContext = null;
            DataContext = skinItems;
        }

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            Skin skinitem = ItemListbox.SelectedItem as Skin;

            if (skinitem != null)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).comboboxItems.SelectedItem = skinitem;
                filtertxt.Text = "";
                ICollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ItemListbox.ItemsSource);

                if (itemsViewOriginal != null)
                    itemsViewOriginal.Filter = null;

                ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.ExitItemsModal();
            }
        }
    }
}

using RustSkinsEditor.Models;
using RustSkinsEditor.Models.Plugins;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
                        ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.DeleteItem(ItemListbox.SelectedItem as BaseItem);
                    }
                }
            }
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                BaseItem baseItem = (sender as StackPanel).DataContext as BaseItem;

                if (baseItem != null)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).comboboxItems.SelectedItem = baseItem;
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
                    if (((BaseItem)o).Name.ToLower().Contains(filtertxt.Text.Trim().ToLower())) return true;
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
                BaseItem baseItem = (BaseItem)((MenuItem)sender).CommandParameter;

                ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.DeleteItem(baseItem);
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
        }

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            BaseItem baseItem = ItemListbox.SelectedItem as BaseItem;

            if (baseItem != null)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).comboboxItems.SelectedItem = baseItem;
                filtertxt.Text = "";
                ICollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ItemListbox.ItemsSource);

                if (itemsViewOriginal != null)
                    itemsViewOriginal.Filter = null;

                ((MainWindow)System.Windows.Application.Current.MainWindow).viewModel.ExitItemsModal();
            }
        }
    }
}

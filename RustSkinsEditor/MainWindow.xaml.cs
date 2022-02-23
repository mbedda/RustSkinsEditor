using Microsoft.Win32;
using RustSkinsEditor.Helpers;
using RustSkinsEditor.Models;
using RustSkinsEditor.ViewModels;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
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

namespace RustSkinsEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel viewModel;
        public string steamApiKey = "26427C873F5956853D7646EB9A21DCB2";

        public MainWindow()
        {
            viewModel = new MainViewModel();

            InitializeComponent();

            DataContext = viewModel;
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {

        }

        public async void FetchSteamSkins(Skin item)
        {
            List<ulong> skinlist = new List<ulong>();

            foreach (var skin in item.Skins)
            {
                ulong skincode = Convert.ToUInt64(skin);
                if (skincode != 0)
                {
                    skinlist.Add(skincode);
                }
            }

            if (skinlist.Count > 0)
            {
                var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync(steamApiKey, skinlist);

                List<SkinDetails> skinsDetails = new List<SkinDetails>();

                foreach (var fileDetails in fileDataDetails)
                {
                    if (fileDetails.Title == null || fileDetails.Title == "")
                        fileDetails.Title = fileDetails.PublishedFileId + "";

                    SkinDetails skinDetails = new SkinDetails();

                    skinDetails.shortname = ((Skin)comboboxItems.SelectedItem).ItemShortname;
                    skinDetails.Title = fileDetails.Title;
                    skinDetails.Description = fileDetails.Description;
                    skinDetails.Code = fileDetails.PublishedFileId;
                    skinDetails.PreviewUrl = fileDetails.PreviewUrl;
                    skinDetails.WorkshopUrl = new Uri("https://steamcommunity.com/sharedfiles/filedetails/?id=" + skinDetails.Code);
                    skinsDetails.Add(skinDetails);
                }

                itemSkinsControl.DataContext = skinsDetails;
            }
        }

        private void comboboxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex > -1)
            {
                FetchSteamSkins((Skin)((ComboBox)sender).SelectedItem);
            }
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                itemSkinsControl.DataContext = null;
                viewModel.LoadFile(FilePathTB.Text);
            }
        }

        private void ExportFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
                viewModel.Save(saveFileDialog.FileName);
        }

        private void AddSkin_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SkinsFile == null || viewModel.SkinsFile.SkinsRoot == null)
            {
                MessageBox.Show("No skins file loaded.");
                return;
            }
            if (comboboxItems.SelectedIndex == -1)
            {
                MessageBox.Show("No item selected.");
                return;
            }

            ulong skincode = 0;

            if (AddSkinTB.Text.Contains("steamcommunity.com"))
            {
                Dictionary<String, String> params1 = new Dictionary<string, string>();
                string[] queryParams = AddSkinTB.Text.Split('?')[1].Split('&');
                foreach (string s in queryParams)
                {
                    string[] queryParameter = s.Split('=');
                    params1.Add(queryParameter[0], queryParameter[1]);
                }

                try
                {
                    if (params1.ContainsKey("id"))
                        skincode = Convert.ToUInt32(params1["id"]);
                }
                catch
                { }
            }
            else
            {
                ulong.TryParse(AddSkinTB.Text, out skincode);
            }

            if (skincode != 0)
            {
                if (viewModel.Add(comboboxItems.SelectedItem as Skin, skincode))
                {
                    itemSkinsControl.DataContext = null;
                    FetchSteamSkins((Skin)(comboboxItems.SelectedItem));
                    AddSkinTB.Text = "";

                    MessageBox.Show("Added Skin " + skincode + " Successfully!");
                }
                else
                {
                    AddSkinTB.Text = "";

                    MessageBox.Show("Skin already exists!");
                }

            }
        }

        private void Fullscreen_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.ExitFullscreen();
        }
        private void ItemList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void SelectItem_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenItemsModal();
        }
    }
}

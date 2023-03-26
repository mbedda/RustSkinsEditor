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
using System.Reflection;
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
using RustSkinsEditor.Models.Plugins;
using System.Collections.ObjectModel;

namespace RustSkinsEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainViewModel();

            InitializeComponent();

            DataContext = viewModel;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            Title = Title + " - v" + version.Major + "." + version.Minor + "." + version.Build.ToString();
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {

        }

        public async void FetchSteamSkins(Skin item)
        {
            List<ulong> skinlist = new List<ulong>();

            viewModel.SelectedItem = item;

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
                var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync("whatever", skinlist);

                List<SteamSkinDetails> skinsDetails = new List<SteamSkinDetails>();

                foreach (var fileDetails in fileDataDetails)
                {
                    if (fileDetails.Title == null || fileDetails.Title == "")
                        fileDetails.Title = fileDetails.PublishedFileId + "";

                    SteamSkinDetails skinDetails = new SteamSkinDetails();

                    skinDetails.shortname = ((Skin)comboboxItems.SelectedItem).ItemShortname;
                    skinDetails.Title = fileDetails.Title;
                    skinDetails.Description = fileDetails.Description;
                    skinDetails.Code = fileDetails.PublishedFileId;
                    skinDetails.PreviewUrl = fileDetails.PreviewUrl;
                    skinDetails.WorkshopUrl = new Uri("https://steamcommunity.com/sharedfiles/filedetails/?id=" + skinDetails.Code);
                    skinDetails.Tags = fileDetails.Tags.ToList();
                    skinsDetails.Add(skinDetails);
                }

                viewModel.ResetSkinsCollection(skinsDetails);
                //itemSkinsControl.DataContext = skinsDetails;
            }
        }

        public void UpdateItemSkinsControlIfComboSelected()
        {
            if (comboboxItems.SelectedIndex > -1)
            {
                //itemSkinsControl.DataContext = null;
                FetchSteamSkins((Skin)(comboboxItems.SelectedItem));
                AddSkinTB.Text = "";
            }
            AddSkinTB.Text = "";
        }

        private void comboboxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex > -1)
            {
                viewModel.SelectedItemSkins = null;
                FetchSteamSkins((Skin)((ComboBox)sender).SelectedItem);
            }
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ContextMenu contextMenu = button.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }

        private void ExportFile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ContextMenu contextMenu = button.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }

        private void AddSkin_Click(object sender, RoutedEventArgs e)
        {
            if (collectionCB.IsChecked.HasValue && collectionCB.IsChecked.Value)
            {
                AddWorkshopSkinCollection();
            }
            else
            {
                AddWorkshopSkinAsync();
            }
        }

        private async void AddWorkshopSkinAsync()
        {
            if (viewModel.SkinsFile == null || viewModel.SkinsFile.SkinsRoot == null)
            {
                MessageBox.Show("No skins file loaded.");
                return;
            }
            //if (comboboxItems.SelectedIndex == -1)
            //{
            //    MessageBox.Show("No item selected.");
            //    return;
            //}

            ulong skincode = 0;

            if (AddSkinTB.Text.Contains("steamcommunity.com") && AddSkinTB.Text.Split('?').Count()>1)
            {
                Dictionary<String, String> params1 = new Dictionary<string, string>();
                string[] queryParams = AddSkinTB.Text.Split('?')[1].Split('&');
                foreach (string s in queryParams)
                {
                    string[] queryParameter = s.Split('=');
                    if(queryParameter.Length < 2)
                        continue;
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
                string shortname = await GetSkinShortnameFromSteamApi(skincode);

                if (string.IsNullOrEmpty(shortname))
                {
                    if (comboboxItems.SelectedIndex == -1)
                    {
                        MessageBox.Show("Could not retrieve skin shortname.");
                        return;
                    }
                    else
                    {
                        shortname = (comboboxItems.SelectedItem as Skin).ItemShortname;
                    }
                }

                if (viewModel.Add(shortname, skincode))
                {
                    UpdateItemSkinsControlIfComboSelected();

                    MessageBox.Show("Added Skin " + skincode + " Successfully!");
                }
                else
                {
                    AddSkinTB.Text = "";

                    MessageBox.Show("Skin already exists!");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid url or skin id.");
            }
        }

        public async Task<string> GetSkinShortnameFromSteamApi(ulong skinid)
        {
            var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync("whatever", new List<ulong>() { skinid });

            if (fileDataDetails == null) return "";

            foreach (var fileDetails in fileDataDetails)
            {
                if (fileDetails.Title == null || fileDetails.Title == "")
                    fileDetails.Title = fileDetails.PublishedFileId + "";

                if (fileDetails.Tags == null) return "";

                string shortname = SteamModels.GetShortnameFromWorkshopTags(fileDetails.Tags.ToList());

                if (string.IsNullOrEmpty(shortname)) return "";

                return shortname;
            }

            return "";
        }

        private void AddWorkshopSkinCollection()
        {
            if (viewModel.SkinsFile == null || viewModel.SkinsFile.SkinsRoot == null)
            {
                MessageBox.Show("No skins file loaded.");
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
                GetCollectionAndAddData(new List<ulong>() { skincode });
            }
        }

        public async void GetCollectionAndAddData(List<ulong> collections)
        {
            var collectionDataDetails = await SteamApi.GetCollectionDetailsAsync(collections);
            List<ulong> CollectionSkins = new List<ulong>();

            if (collectionDataDetails != null && collectionDataDetails.Response != null && collectionDataDetails.Response.CollectionDetails != null)
            {
                foreach (var collection in collectionDataDetails.Response.CollectionDetails)
                {
                    if (collection.Children != null)
                    {
                        foreach (var item in collection.Children)
                        {
                            CollectionSkins.Add(Convert.ToUInt32(item.PublishedFileId));
                        }
                    }
                }
            }

            if (CollectionSkins.Count > 0)
            {
                bool partialfail = false;
                bool partialsuccess = false;

                var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync("whatever", CollectionSkins);

                foreach (var fileDetails in fileDataDetails)
                {
                    if (fileDetails.Title == null || fileDetails.Title == "")
                        fileDetails.Title = fileDetails.PublishedFileId + "";

                    if (fileDetails.Tags == null)
                    {
                        partialfail = true;
                        continue;
                    }

                    string shortname = SteamModels.GetShortnameFromWorkshopTags(fileDetails.Tags.ToList());

                    if (string.IsNullOrEmpty(shortname))
                    {
                        partialfail = true;
                        continue;
                    }

                    if (viewModel.Add(shortname, fileDetails.PublishedFileId))
                    {
                        partialsuccess = true;
                    }
                    else { partialfail = true; }
                }

                if(partialsuccess && !partialfail)
                {
                    UpdateItemSkinsControlIfComboSelected();
                    MessageBox.Show("Added Skins in Workshop Collection " + collections[0] + " Successfully!");
                }
                else if(partialsuccess && partialfail)
                {
                    UpdateItemSkinsControlIfComboSelected();
                    MessageBox.Show("Added Some Skins in Workshop Collection " + collections[0] + " Successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to Add Skins in Workshop Collection " + collections[0] + "!");
                }
            }
            else
            {
                MessageBox.Show("Collection was invalid or empty!");
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

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Border rectangle = sender as Border;
                ContextMenu contextMenu = rectangle.ContextMenu;
                //contextMenu.PlacementTarget = rectangle;
                //contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                contextMenu.IsOpen = true;
            }
        }

        private void ChangeSteamAPIKey_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowConfigEditor();
        }

        private void ConfigEditor_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.HideConfigEditor();
        }

        private void SkinsLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                //itemSkinsControl.DataContext = null;
                viewModel.LoadFile(FilePathTB.Text, SkinFileSource.Skins);
            }
        }

        private void SkinnerLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                //itemSkinsControl.DataContext = null;
                viewModel.LoadFile(FilePathTB.Text, SkinFileSource.Skinner);
            }
        }

        private void SkinBoxLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                //itemSkinsControl.DataContext = null;
                viewModel.LoadFile(FilePathTB.Text, SkinFileSource.SkinBox);
            }
        }

        private void SkinsSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
                viewModel.Save(saveFileDialog.FileName, SkinFileSource.Skins);
        }

        private void SkinnerSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Save("", SkinFileSource.Skinner);
        }

        private void SkinBoxSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Save("", SkinFileSource.SkinBox);
        }

        private void SkinnerJSONOn_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.HideSkinnerJSON();
        }

        private void CopySkinnerJSONText_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(viewModel.SkinsFile.SkinnerJSONString);
        }

        private void SkinBoxJSONOn_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.HideSkinBoxJSON();
        }

        private void CopySkinBoxJSONText_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(viewModel.SkinsFile.SkinBoxJSONString);
        }
    }
}

using Microsoft.Win32;
using RustSkinsEditor.Helpers;
using RustSkinsEditor.Models;
using RustSkinsEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RustSkinsEditor.Models.Plugins;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        public async void FetchSteamSkins(BaseItem item)
        {
            if(item == null || item.Skins == null) return;

            List<ulong> skinlist = new List<ulong>();

            viewModel.SelectedItem = item;

            foreach (var baseSkin in item.Skins)
            {
                if (baseSkin.PreviewUrl != null) continue;

                if (baseSkin.WorkshopId != 0)
                {
                    skinlist.Add(baseSkin.WorkshopId);
                }
            }

            if (skinlist.Count > 0)
            {
                var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync("whatever", skinlist);

                foreach (var fileDetails in fileDataDetails)
                {
                    if (fileDetails.Title == null || fileDetails.Title == "")
                        fileDetails.Title = fileDetails.PublishedFileId + "";

                    foreach (var baseSkin in viewModel.SelectedItem.Skins)
                    {
                        if(baseSkin.WorkshopId == fileDetails.PublishedFileId)
                        {
                            baseSkin.Name = fileDetails.Title;
                            baseSkin.PreviewUrl = fileDetails.PreviewUrl;
                            baseSkin.WorkshopUrl = new Uri("https://steamcommunity.com/sharedfiles/filedetails/?id=" + baseSkin.WorkshopId);
                        }
                    }
                }

                //viewModel.ResetSkinsCollection(skinsDetails);
                //itemSkinsControl.DataContext = skinsDetails;
            }
        }

        public void UpdateItemSkinsControlIfComboSelected()
        {
            if (comboboxItems.SelectedIndex > -1)
            {
                //itemSkinsControl.DataContext = null;
                FetchSteamSkins((BaseItem)(comboboxItems.SelectedItem));
                AddSkinTB.Text = "";
            }
            AddSkinTB.Text = "";
        }

        private void comboboxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex > -1)
            {
                viewModel.SelectedItem = null;
                FetchSteamSkins((BaseItem)((ComboBox)sender).SelectedItem);
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
            if (viewModel.SkinsFile == null || viewModel.SkinsFile.BaseModel == null)
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
                string shortname = "";

                if (comboboxItems.SelectedIndex != -1)
                {
                    shortname = (comboboxItems.SelectedItem as BaseItem).Shortname;
                }
                else
                {
                    shortname = await GetSkinShortnameFromSteamApi(skincode);

                    if (string.IsNullOrEmpty(shortname))
                    {
                        MessageBox.Show("Could not retrieve skin shortname.");
                        return;
                    }
                }
                

                if (viewModel.AddSkin(shortname, skincode))
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
            if (viewModel.SkinsFile == null || viewModel.SkinsFile.BaseModel == null)
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

                    if (viewModel.AddSkin(shortname, fileDetails.PublishedFileId))
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

        private void SkinnerBetaLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                //itemSkinsControl.DataContext = null;
                viewModel.LoadFile(FilePathTB.Text, SkinFileSource.SkinnerBeta);
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

        private void LSkinsLoad_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FilePathTB.Text = dialog.FileName;
                viewModel.LoadFolder(FilePathTB.Text, SkinFileSource.LSkins);
            }
        }

        private void SkinControllerLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                //itemSkinsControl.DataContext = null;
                viewModel.LoadFile(FilePathTB.Text, SkinFileSource.SkinController);
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

        private void SkinnerBetaSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
                viewModel.Save(saveFileDialog.FileName, SkinFileSource.SkinnerBeta);
        }

        private void SkinBoxSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Save("", SkinFileSource.SkinBox);
        }

        private void SkinnerJSONOn_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.HideSkinnerJSON();
        }

        private void LSkinsSave_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                viewModel.Save(dialog.FileName, SkinFileSource.LSkins);
            }
        }

        private void SkinControllerSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
                viewModel.Save(saveFileDialog.FileName, SkinFileSource.SkinController);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadGameItems();
        }


        private void DeleteMarketSkins_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            foreach (var baseItem in viewModel.SkinsFile.BaseModel.Items)
            {
                for (int i = baseItem.Skins.Count - 1; i >= 0; i--)
                {
                    if (viewModel.RustItems.DLCsData.ProhibitedSkins.Contains(baseItem.Skins[i].WorkshopId) 
                        || viewModel.RustItems.DLCsData.ProhibitedSkinsItemIds.Contains(baseItem.Skins[i].WorkshopId.ToString()))
                    {
                        baseItem.Skins.RemoveAt(i);
                        count++;
                    }
                }
            }
            viewModel.UpdateActivity();

            if(viewModel.SelectedItem != null)
            {
                viewModel.SelectedItem = null;
                FetchSteamSkins((BaseItem)(comboboxItems.SelectedItem));
            }

            MessageBox.Show($"Deleted {count} market skins");
        }
    }
}

using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using RustSkinsEditor.Models;
using RustSkinsEditor.Models.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using IOPath = System.IO.Path;

namespace RustSkinsEditor.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private SkinsFile skinsFile;
        public SkinsFile SkinsFile
        {
            get { return skinsFile; }
            set { SetProperty(ref skinsFile, value); }
        }

        private BaseItem selectedItem;
        public BaseItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        private string activity;
        public string Activity
        {
            get { return activity; }
            set { SetProperty(ref activity, value); }
        }

        private bool itemsModal;
        public bool ItemsModal
        {
            get { return itemsModal; }
            set { SetProperty(ref itemsModal, value); }
        }

        private bool fullscreen;
        public bool Fullscreen
        {
            get { return fullscreen; }
            set { SetProperty(ref fullscreen, value); }
        }

        private bool _ConfigEditorOn;
        public bool ConfigEditorOn
        {
            get { return _ConfigEditorOn; }
            set { SetProperty(ref _ConfigEditorOn, value); }
        }

        private bool partialLoadingScreen;
        public bool PartialLoadingScreen
        {
            get { return partialLoadingScreen; }
            set { SetProperty(ref partialLoadingScreen, value); }
        }

        private bool fullLoadingScreen;
        public bool FullLoadingScreen
        {
            get { return fullLoadingScreen; }
            set { SetProperty(ref fullLoadingScreen, value); }
        }

        private bool _SkinnerJSONOn;
        public bool SkinnerJSONOn
        {
            get { return _SkinnerJSONOn; }
            set { SetProperty(ref _SkinnerJSONOn, value); }
        }

        private bool _SkinBoxJSONOn;
        public bool SkinBoxJSONOn
        {
            get { return _SkinBoxJSONOn; }
            set { SetProperty(ref _SkinBoxJSONOn, value); }
        }

        private Uri fullscreenImage;
        public Uri FullscreenImage
        {
            get { return fullscreenImage; }
            set { SetProperty(ref fullscreenImage, value); }
        }

        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand ExitFullscreenCommand { get; set; }

        public RustItems RustItems { get; set; }
        public List<string> RustItemsList { get; set; }

        private Config _config;
        public Config Config
        {
            get { return _config; }
            set { SetProperty(ref _config, value); }
        }

        string DataPath = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RustSkinsEditor", "config.json");

        public string SteamPath { get; set; }

        public MainViewModel()
        {
            LoadConfig();
            UpdateCommand = new DelegateCommand(Update);
            ExitFullscreenCommand = new DelegateCommand(ExitFullscreen);

            RustItems = new RustItems();

            GetSteamPath();

            Fullscreen = false;
            PartialLoadingScreen = true;
            FullLoadingScreen = false;

            Activity = "No file loaded...";
        }

        public async Task LoadGameItems()
        {
            FullLoadingScreen = true;
            await RustItems.Load(SteamPath);
            RustItemsList = RustItems.Items.Select(x => x.shortName).ToList();

            FullLoadingScreen = false;
        }

        public void GetSteamPath()
        {
            SteamPath = "";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");

            if (key != null)
            {
                SteamPath = key.GetValue("InstallPath")?.ToString();
                key.Close();
            }
            else
            {
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");

                if (key != null)
                {
                    SteamPath = key.GetValue("InstallPath")?.ToString();
                    key.Close();
                }
            }
            if (string.IsNullOrEmpty(SteamPath))
            {
                SteamPath = "";
                return;
            }

            string libfoldersPath = Path.Combine(SteamPath, "steamapps", "libraryfolders.vdf");
            string driveRegex = @"[A-Z]:\\";

            if (File.Exists(libfoldersPath))
            {
                string[] configLines = File.ReadAllLines(libfoldersPath);
                foreach (var item in configLines)
                {
                    Match match = Regex.Match(item, driveRegex);
                    if (item != string.Empty && match.Success)
                    {
                        string matched = match.ToString();
                        string item2 = item.Substring(item.IndexOf(matched));
                        item2 = item2.Replace("\\\\", "\\");
                        item2 = item2.Replace("\"", "");

                        string pat = Path.Combine(item2, "steamapps", "common", "Rust", "Rust.exe");

                        if (File.Exists(pat))
                        {
                            SteamPath = item2;
                            break;
                        }
                    }
                }
            }
        }

        //private void SelectedItemSkins_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    var skin = SkinsFile.SkinsRoot.Skins.FirstOrDefault(s => s.ItemShortname == SelectedItem.ItemShortname);
        //    skin.Skins.Clear();
        //    skin.Skins.AddRange(SelectedItemSkins.Select(s => s.Code).ToList());
        //    UpdateActivity();
        //}

        //public void ResetSkinsCollection(IEnumerable<SteamSkinDetails> skinlist)
        //{
        //    SelectedItemSkins = new ObservableCollection<SteamSkinDetails>(skinlist);
        //    SelectedItemSkins.CollectionChanged += SelectedItemSkins_CollectionChanged;
        //}

        public void LoadConfig()
        {
            //Config = Common.LoadJson<Config>(DataPath);

            //if(Config == null) Config = new Config();
        }

        public void SaveConfig()
        {
            //Common.SaveJson(Config, DataPath);
        }

        public async Task InitAsync()
        {

            //Update();
        }

        public void OpenItemsModal()
        {
            ItemsModal = true;
        }

        public void ExitItemsModal()
        {
            ItemsModal = false;
        }

        public void ShowConfigEditor()
        {
            ConfigEditorOn = true;
        }
        public void HideConfigEditor()
        {
            SaveConfig();
            ConfigEditorOn = false;
        }

        public void ShowSkinnerJSON()
        {
            SkinnerJSONOn = true;
        }
        public void HideSkinnerJSON()
        {
            SkinnerJSONOn = false;
        }

        public void ShowSkinBoxJSON()
        {
            SkinBoxJSONOn = true;
        }
        public void HideSkinBoxJSON()
        {
            SkinBoxJSONOn = false;
        }

        public void SetFullscreen(Uri image)
        {
            FullscreenImage = image;
            Fullscreen = true;
        }

        public void ExitFullscreen()
        {
            Fullscreen = false;
        }

        public void LoadFile(string filepath, SkinFileSource skinFileSource)
        {
            SelectedItem = null;
            SkinsFile = new SkinsFile();
            SkinsFile.LoadFile(filepath, skinFileSource);

            if (SkinsFile.BaseModel != null && SkinsFile.BaseModel.Items != null)
            {
                foreach (var baseItem in SkinsFile.BaseModel.Items)
                {
                    RustItem rustItem = RustItems.GetRustItem(baseItem.Shortname);

                    if (rustItem != null)
                        baseItem.Name = rustItem.displayName;
                    else
                        baseItem.Name = baseItem.Shortname;
                }
            }

            UpdateActivity();
        }

        public void LoadFolder(string folderpath, SkinFileSource skinFileSource)
        {
            SelectedItem = null;
            SkinsFile = new SkinsFile();
            SkinsFile.LoadFiles(folderpath, skinFileSource);

            if (SkinsFile.BaseModel != null && SkinsFile.BaseModel.Items != null)
            {
                foreach (var baseItem in SkinsFile.BaseModel.Items)
                {
                    RustItem rustItem = RustItems.GetRustItem(baseItem.Shortname);

                    if (rustItem != null)
                        baseItem.Name = rustItem.displayName;
                    else
                        baseItem.Name = baseItem.Shortname;
                }
            }

            UpdateActivity();
        }

        public void UpdateActivity()
        {
            if (SkinsFile.BaseModel == null || SkinsFile.BaseModel.Items == null)
            {
                SkinsFile.BaseModel = new BaseModel();
                Activity = "No file loaded...";
            }
            else
            {
                Activity = "Skins file imported.. " + SkinsFile.BaseModel.Items.Count() + " items, " + SkinsFile.BaseModel.Items.SelectMany(x => x.Skins).Count() + " skins";
                PartialLoadingScreen = false;
            }
        }

        private void Update()
        {
        }

        public async void Save(string filepath, SkinFileSource skinFileSource)
        {
            if (SkinsFile != null)
            {
                switch (skinFileSource)
                {
                    case SkinFileSource.Skins:
                        FullLoadingScreen = true;
                        SkinsFile.SaveFile(filepath, Config, skinFileSource);
                        FullLoadingScreen = false;
                        break;
                    case SkinFileSource.Skinner:
                        FullLoadingScreen = true;
                        await SkinsFile.GetSkinnerJSONString();
                        ShowSkinnerJSON();
                        FullLoadingScreen = false;
                        break;
                    case SkinFileSource.SkinBox:
                        FullLoadingScreen = true;
                        await SkinsFile.GetSkinBoxJSONString();
                        ShowSkinBoxJSON();
                        FullLoadingScreen = false;
                        break;
                    case SkinFileSource.LSkins:
                        FullLoadingScreen = true;
                        SkinsFile.SaveFiles(filepath, Config, skinFileSource);
                        FullLoadingScreen = false;
                        break;
                }
            }
        }

        public bool AddSkin(string shortname, ulong skincode)
        {
            if(SkinsFile != null && SkinsFile.BaseModel != null && SkinsFile.BaseModel.Items != null)
            {
                var baseItem = SkinsFile.BaseModel.Items.FirstOrDefault(s => s.Shortname == shortname);

                if (baseItem == null)
                {
                    baseItem = new BaseItem() { Shortname = shortname };

                    var rustItem = RustItems.GetRustItem(shortname);
                    if (rustItem != null)
                        baseItem.Name = rustItem.displayName;

                    SkinsFile.BaseModel.Items.Add(baseItem);
                }

                if (baseItem.Skins.Where(s=>s.WorkshopId == skincode).Count() == 0)
                {
                    baseItem.Skins.Add(new BaseSkin()
                    {
                        WorkshopId = skincode,
                        Name = skincode.ToString()
                    });
                    UpdateActivity();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool AddItem(string Shortname)
        {
            if (SkinsFile != null && SkinsFile.BaseModel != null && SkinsFile.BaseModel.Items != null)
            {
                var baseItem = SkinsFile.BaseModel.Items.FirstOrDefault(s => s.Shortname == Shortname);
                if (baseItem == null)
                {
                    baseItem = new BaseItem() { Shortname = Shortname };

                    var rustItem = RustItems.GetRustItem(Shortname);
                    if (rustItem != null)
                        baseItem.Name = rustItem.displayName;

                    SkinsFile.BaseModel.Items.Add(baseItem);

                    UpdateActivity();
                    return true;
                }
                else
                {
                    MessageBox.Show("Item already exists!");
                    return false;
                }
            }
            return false;
        }

        public void DeleteSkin(string shortname, ulong skincode)
        {
            var baseItem = SkinsFile.BaseModel.Items.FirstOrDefault(s => s.Shortname == shortname);

            if(baseItem != null)
            {
                var baseSkin = baseItem.Skins.FirstOrDefault(s => s.WorkshopId == skincode);

                if (baseItem != null)
                {
                    baseItem.Skins.Remove(baseSkin);
                    UpdateActivity();
                }
            }
        }

        public void DeleteItem(string shortname)
        {
            var baseItem = SkinsFile.BaseModel.Items.FirstOrDefault(s => s.Shortname == shortname);

            if (baseItem != null)
            {
                SkinsFile.BaseModel.Items.Remove(baseItem);
                UpdateActivity();
            }
        }

        public void DeleteItem(BaseItem baseItem)
        {
            if (baseItem != null)
            {
                SkinsFile.BaseModel.Items.Remove(baseItem);
                UpdateActivity();
            }
        }
    }
}

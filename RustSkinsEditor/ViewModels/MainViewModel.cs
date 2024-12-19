using Prism.Commands;
using Prism.Mvvm;
using RustSkinsEditor.Models;
using RustSkinsEditor.Models.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private Skin selectedItem;
        public Skin SelectedItem
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

        private ObservableCollection<SteamSkinDetails> _SelectedItemSkins;
        public ObservableCollection<SteamSkinDetails> SelectedItemSkins
        {
            get { return _SelectedItemSkins; }
            set { SetProperty(ref _SelectedItemSkins, value); }
        }

        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand ExitFullscreenCommand { get; set; }

        public RustItems RustItems { get; set; }

        private Config _config;
        public Config Config
        {
            get { return _config; }
            set { SetProperty(ref _config, value); }
        }

        string DataPath = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RustSkinsEditor", "config.json");

        public MainViewModel()
        {
            LoadConfig();
            UpdateCommand = new DelegateCommand(Update);
            ExitFullscreenCommand = new DelegateCommand(ExitFullscreen);

            RustItems = new RustItems();

            Fullscreen = false;
            PartialLoadingScreen = true;
            FullLoadingScreen = false;

            Activity = "No file loaded...";
        }

        private void SelectedItemSkins_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var skin = SkinsFile.SkinsRoot.Skins.FirstOrDefault(s => s.ItemShortname == SelectedItem.ItemShortname);
            skin.Skins.Clear();
            skin.Skins.AddRange(SelectedItemSkins.Select(s => s.Code).ToList());
            UpdateActivity();
        }

        public void ResetSkinsCollection(IEnumerable<SteamSkinDetails> skinlist)
        {
            SelectedItemSkins = new ObservableCollection<SteamSkinDetails>(skinlist);
            SelectedItemSkins.CollectionChanged += SelectedItemSkins_CollectionChanged;
        }

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
            SelectedItemSkins = null;
            SkinsFile = new SkinsFile();
            SkinsFile.LoadFile(filepath, skinFileSource);

            if (SkinsFile.SkinsRoot != null && SkinsFile.SkinsRoot.Skins != null)
            {
                //RustItems.ParseData(SkinsFile.SkinsRoot.Skins.Select(s => s.ItemShortname).ToList());

                foreach (var skinitem in SkinsFile.SkinsRoot.Skins)
                {
                    RustItem rustItem = RustItems.GetRustItem(skinitem.ItemShortname);

                    if (rustItem != null)
                    {
                        skinitem.Name = rustItem.displayName;
                        skinitem.ImagePath = new Uri("https://rustlabs.com/img/items180/" + rustItem.shortName + ".png");
                    }
                    else
                    {
                        skinitem.Name = skinitem.ItemShortname;
                        skinitem.ImagePath = new Uri("https://i.imgur.com/nY1CFCC.png");
                    }
                }
            }

            UpdateActivity();
        }

        public void UpdateActivity()
        {
            if (SkinsFile.SkinsRoot == null || SkinsFile.SkinsRoot.Skins == null)
            {
                SkinsFile.SkinsRoot = new SkinsRoot();
                Activity = "No file loaded...";
            }
            else
            {
                Activity = "Skins file imported.. " + SkinsFile.SkinsRoot.Skins.Count() + " items, " + SkinsFile.SkinsRoot.Skins.SelectMany(x => x.Skins).Count() + " skins";
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
                }
            }
        }

        public bool Add(Skin skinitem, ulong skincode)
        {
            if (!skinitem.Skins.Contains(skincode))
            {
                skinitem.Skins.Add(skincode);
                UpdateActivity();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Add(string shortname, ulong skincode)
        {
            if(SkinsFile != null && SkinsFile.SkinsRoot != null && SkinsFile.SkinsRoot.Skins!=null)
            {
                var skinitem = SkinsFile.SkinsRoot.Skins.FirstOrDefault(s => s.ItemShortname == shortname);

                if (skinitem == null)
                {
                    SkinsFile.SkinsRoot.Skins.Add(new Skin() { ItemShortname = shortname });
                    skinitem = SkinsFile.SkinsRoot.Skins.FirstOrDefault(s => s.ItemShortname == shortname);
                }

                if (!skinitem.Skins.Contains(skincode))
                {
                    skinitem.Skins.Add(skincode);
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
            if (SkinsFile != null && SkinsFile.SkinsRoot != null && SkinsFile.SkinsRoot.Skins != null)
            {
                var skinitem = SkinsFile.SkinsRoot.Skins.Where(x => x.ItemShortname == Shortname).FirstOrDefault();
                if (skinitem == null)
                {
                    SkinsFile.SkinsRoot.Skins.Add(new Skin() { Name = Shortname, ItemShortname = Shortname, ImagePath = new Uri("https://i.imgur.com/nY1CFCC.png") });
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

        public void Delete(string shortname, ulong skincode)
        {
            var skin = SkinsFile.SkinsRoot.Skins.FirstOrDefault(s => s.ItemShortname == shortname);
            skin.Skins.Remove(skincode);
            UpdateActivity();
        }
    }
}

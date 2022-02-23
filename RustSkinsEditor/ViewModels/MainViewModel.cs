using Prism.Commands;
using Prism.Mvvm;
using RustSkinsEditor.Helpers;
using RustSkinsEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private Uri fullscreenImage;
        public Uri FullscreenImage
        {
            get { return fullscreenImage; }
            set { SetProperty(ref fullscreenImage, value); }
        }

        public DelegateCommand<string> LoadFileCommand { get; set; }
        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand ExitFullscreenCommand { get; set; }

        public RustItems RustItems { get; set; }

        public MainViewModel()
        {
            LoadFileCommand = new DelegateCommand<string>(LoadFile);
            UpdateCommand = new DelegateCommand(Update);
            ExitFullscreenCommand = new DelegateCommand(ExitFullscreen);

            RustItems = new RustItems();

            Fullscreen = false;

            Activity = "No file loaded...";
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

        public void SetFullscreen(Uri image)
        {
            FullscreenImage = image;
            Fullscreen = true;
        }

        public void ExitFullscreen()
        {
            Fullscreen = false;
        }

        public void LoadFile(string filepath)
        {
            SkinsFile = new SkinsFile();
            SkinsFile.SkinsRoot = Common.LoadJson<SkinsRoot>(filepath);

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
            }
        }

        private void Update()
        {
        }

        public void Save(string filepath)
        {
            //if (DataChanged)
            //{
            Common.SaveJson(SkinsFile.SkinsRoot, filepath);
            //}
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

        public bool AddItem(string Shortname)
        {
            if (SkinsFile != null && SkinsFile.SkinsRoot != null && SkinsFile.SkinsRoot.Skins != null)
            {
                var skinitem = SkinsFile.SkinsRoot.Skins.Find(x => x.ItemShortname == Shortname);
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

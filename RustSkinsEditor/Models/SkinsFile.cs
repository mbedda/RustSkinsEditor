using Prism.Mvvm;
using RustSkinsEditor.Helpers;
using RustSkinsEditor.Models.Plugins;
using RustSkinsEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RustSkinsEditor.Models
{

    public enum SkinFileSource
    {
        Skins,
        Skinner,
        SkinBox,
        LSkins
    }

    public class SkinsFile : BindableBase
    {
        private SkinsRoot skinsRoot;
        public SkinsRoot SkinsRoot
        {
            get { return skinsRoot; }
            set { SetProperty(ref skinsRoot, value); }
        }

        private SkinnerRoot skinnerRoot;
        public SkinnerRoot SkinnerRoot
        {
            get { return skinnerRoot; }
            set { SetProperty(ref skinnerRoot, value); }
        }

        private SkinBoxRoot skinBoxRoot;
        public SkinBoxRoot SkinBoxRoot
        {
            get { return skinBoxRoot; }
            set { SetProperty(ref skinBoxRoot, value); }
        }

        private LSkinsRoot lSkinsRoot;
        public LSkinsRoot LSkinsRoot
        {
            get { return lSkinsRoot; }
            set { SetProperty(ref lSkinsRoot, value); }
        }

        public void LoadFile(string filepath, SkinFileSource skinFileSource = SkinFileSource.Skins)
        {
            switch (skinFileSource)
            {
                case SkinFileSource.Skins:
                    SkinsRoot = Common.LoadJson<SkinsRoot>(filepath);
                    break;
                case SkinFileSource.Skinner:
                    SkinnerRoot = Common.LoadJson<SkinnerRoot>(filepath);
                    ConvertSkinnerToSkins();
                    break;
                case SkinFileSource.SkinBox:
                    SkinBoxRoot = Common.LoadJson<SkinBoxRoot>(filepath);
                    ConvertSkinBoxToSkins();
                    break;
            }
        }

        public void LoadFiles(string folderpath, SkinFileSource skinFileSource = SkinFileSource.Skins)
        {
            MainViewModel vm = ((MainWindow)Application.Current.MainWindow).viewModel;

            switch (skinFileSource)
            {
                case SkinFileSource.LSkins:
                    if (Directory.Exists(folderpath))
                    {
                        LSkinsRoot = new LSkinsRoot();
                        string[] files = Directory.GetFiles(folderpath);

                        foreach (string file in files)
                        {
                            string shortname = Path.GetFileNameWithoutExtension(file);
                            if (vm.RustItemsList.Contains(Path.GetFileNameWithoutExtension(file)))
                            {
                                LSkinsRoot.LSkinItem lSkinItem = new();
                                lSkinItem.Skins = Common.LoadJson<Dictionary<string, LSkinsRoot.LSkinItemSkin>>(file);
                                LSkinsRoot.Items.Add(shortname, lSkinItem);
                            }
                        }
                    }
                    ConvertLSkinsToSkins();
                    break;
            }
        }

        public void SaveFile(string filepath, Config config, SkinFileSource skinFileSource = SkinFileSource.Skins)
        {
            switch (skinFileSource)
            {
                case SkinFileSource.Skins:
                    Common.SaveJsonNewton<SkinsRoot>(SkinsRoot, filepath);
                    break;
                    //case SkinFileSource.Skinner:
                    //    ConvertSkinsToSkinner();
                    //    FetchSteamSkinsNamesSkinnerAndSave(filepath, config);
                    //    break;
            }
        }

        public void SaveFiles(string folderpath, Config config, SkinFileSource skinFileSource = SkinFileSource.LSkins)
        {
            switch (skinFileSource)
            {
                case SkinFileSource.LSkins:
                    ConvertSkinsToLSkins();
                    //FetchSteamSkinsNamesLSkins();
                    foreach (var item in LSkinsRoot.Items)
                    {
                        Common.SaveJsonNewton<Dictionary<string, LSkinsRoot.LSkinItemSkin>>(item.Value.Skins, Path.Combine(folderpath, item.Key + ".json"));
                    }
                    //Common.SaveJsonNewton<SkinsRoot>(SkinsRoot, filepath);
                    break;
            }
        }

        public void ConvertSkinnerToSkins()
        {
            if (SkinnerRoot != null && skinnerRoot.Skins != null)
            {
                var results = SkinnerRoot.Skins.GroupBy(p => p.Value.itemShortname,
                    p => p.Key, (key, g) => new { Shortname = key, Keys = g.ToList() });

                SkinsRoot SkinsRootTmp = new SkinsRoot();

                foreach (var item in results)
                {
                    Skin skin = new Skin();
                    skin.ItemShortname = item.Shortname;
                    skin.Skins = item.Keys.ToList();

                    SkinsRootTmp.Skins.Add(skin);
                }

                SkinsRoot = SkinsRootTmp;
                //SkinsRoot.Skins.Add
            }
        }

        public void ConvertSkinsToSkinner()
        {
            if (SkinsRoot != null)
            {
                SkinnerRoot = new SkinnerRoot();
                skinnerRoot.Skins = new Dictionary<ulong, SkinnerRoot.SkinnerSkin>();

                foreach (var item in SkinsRoot.Skins)
                {
                    foreach (var skinid in item.Skins)
                    {
                        if (!skinnerRoot.Skins.ContainsKey(skinid))
                        {
                            skinnerRoot.Skins.Add(skinid,
                                new SkinnerRoot.SkinnerSkin() { itemDisplayname = item.Name, itemShortname = item.ItemShortname });
                        }
                    }
                }
            }
        }

        public void ConvertSkinBoxToSkins()
        {
            if (SkinBoxRoot != null && SkinBoxRoot.Skins != null)
            {
                SkinsRoot SkinsRootTmp = new SkinsRoot();

                foreach (var item in SkinBoxRoot.Skins)
                {
                    Skin skin = new Skin();
                    skin.ItemShortname = item.Key;
                    skin.Skins = item.Value.ToList();

                    SkinsRootTmp.Skins.Add(skin);
                }

                SkinsRoot = SkinsRootTmp;
            }
        }

        public void ConvertSkinsToSkinBox()
        {
            if (SkinsRoot != null)
            {
                SkinBoxRoot = new SkinBoxRoot();
                SkinBoxRoot.Skins = new Dictionary<string, List<ulong>>();

                foreach (var item in SkinsRoot.Skins)
                {
                    SkinBoxRoot.Skins.Add(item.ItemShortname, item.Skins);
                }
            }
        }

        public void ConvertLSkinsToSkins()
        {
            if (LSkinsRoot != null && LSkinsRoot.Items != null)
            {
                SkinsRoot SkinsRootTmp = new SkinsRoot();

                foreach (var item in LSkinsRoot.Items)
                {
                    Skin skin = new Skin();
                    skin.ItemShortname = item.Key;
                    foreach (var lskin in item.Value.Skins)
                    {
                        if (lskin.Key == "0") continue;

                        if(UInt64.TryParse(lskin.Key, out ulong skinId))
                            skin.Skins.Add(skinId);
                    }

                    SkinsRootTmp.Skins.Add(skin);
                }

                SkinsRoot = SkinsRootTmp;
            }
        }

        public void ConvertSkinsToLSkins()
        {
            if (SkinsRoot != null)
            {
                LSkinsRoot = new LSkinsRoot();
                LSkinsRoot.Items = new();

                foreach (var item in SkinsRoot.Skins)
                {
                    LSkinsRoot.LSkinItem lSkinItem = new LSkinsRoot.LSkinItem();
                    lSkinItem.Skins = new();

                    lSkinItem.Skins.Add("0", new()
                    {
                        Name = item.Name
                    });

                    foreach (var skin in item.Skins)
                    {
                        lSkinItem.Skins.Add(skin.ToString(),
                            new()
                            {
                                Name = item.Name
                            });
                    }

                    LSkinsRoot.Items.Add(item.ItemShortname, lSkinItem);
                }
            }
        }

        private string _SkinnerJSONString;
        public string SkinnerJSONString
        {
            get { return _SkinnerJSONString; }
            set { SetProperty(ref _SkinnerJSONString, value); }
        }

        public async Task GetSkinnerJSONString()
        {
            ConvertSkinsToSkinner();

            if (SkinnerRoot == null) return;

            List<ulong> skinlist = SkinnerRoot.Skins.Select(s => s.Key).ToList();


            if (skinlist.Count > 0)
            {
                int skip = 0;
                int take = 3000;

                while(skip < skinlist.Count)
                {
                    List<ulong> maxedList = skinlist.Skip(skip).Take(take).ToList();
                    skip += maxedList.Count;

                    var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync("whatever", maxedList);

                    foreach (var fileDetails in fileDataDetails)
                    {
                        if (fileDetails.Title == null || fileDetails.Title == "")
                            fileDetails.Title = fileDetails.PublishedFileId + "";

                        SkinnerRoot.Skins[fileDetails.PublishedFileId].itemDisplayname = fileDetails.Title;
                    }
                }
                

                string skinnerJSON = Common.GetJsonString<SkinnerRoot>(SkinnerRoot);

                if (skinnerJSON != null && skinnerJSON.StartsWith('{') && skinnerJSON.EndsWith('}'))
                {
                    skinnerJSON = Common.FormatJSON(skinnerJSON);
                    skinnerJSON = skinnerJSON.Substring(1, skinnerJSON.Length - 2).Trim();

                    SkinnerJSONString = skinnerJSON;
                }
            }
        }

        private string _SkinBoxJSONString;
        public string SkinBoxJSONString
        {
            get { return _SkinBoxJSONString; }
            set { SetProperty(ref _SkinBoxJSONString, value); }
        }

        public async Task GetSkinBoxJSONString()
        {
            ConvertSkinsToSkinBox();

            if (SkinBoxRoot == null) return;


            string skinBoxJSON = Common.GetJsonString<SkinBoxRoot>(SkinBoxRoot);

            if (skinBoxJSON != null && skinBoxJSON.StartsWith('{') && skinBoxJSON.EndsWith('}'))
            {
                skinBoxJSON = Common.FormatJSON(skinBoxJSON);
                skinBoxJSON = skinBoxJSON.Substring(1, skinBoxJSON.Length - 2).Trim();

                SkinBoxJSONString = skinBoxJSON;
            }
        }

        public async void FetchSteamSkinsNamesSkinnerAndSave(string filepath, Config config)
        {
            if (SkinnerRoot == null) return;

            List<ulong> skinlist = SkinnerRoot.Skins.Select(s => s.Key).ToList();

            if (skinlist.Count > 0)
            {
                int skip = 0;
                int take = 3000;

                while (skip < skinlist.Count)
                {
                    List<ulong> maxedList = skinlist.Skip(skip).Take(take).ToList();
                    skip += maxedList.Count;

                    var fileDataDetails = await SteamApi.GetPublishedFileDetailsAsync("whatever", maxedList);

                    foreach (var fileDetails in fileDataDetails)
                    {
                        if (fileDetails.Title == null || fileDetails.Title == "")
                            fileDetails.Title = fileDetails.PublishedFileId + "";

                        SkinnerRoot.Skins[fileDetails.PublishedFileId].itemDisplayname = fileDetails.Title;
                    }
                }

                Common.SaveJson<SkinnerRoot>(SkinnerRoot, filepath);
            }
        }
    }
}

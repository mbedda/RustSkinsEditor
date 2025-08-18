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
        SkinnerBeta,
        SkinBox,
        LSkins
    }

    public class SkinsFile : BindableBase
    {
        private BaseModel _baseModel;
        public BaseModel BaseModel
        {
            get { return _baseModel; }
            set { SetProperty(ref _baseModel, value); }
        }


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
                    ConverSkinsToBaseModel();
                    break;
                case SkinFileSource.Skinner:
                    SkinnerRoot = Common.LoadJson<SkinnerRoot>(filepath);
                    ConvertSkinnerToBaseModel();
                    break;
                case SkinFileSource.SkinnerBeta:
                    SkinnerRoot = new SkinnerRoot();
                    SkinnerRoot.Skins = Common.LoadJson<Dictionary<ulong, SkinnerRoot.SkinnerSkin>>(filepath);
                    ConvertSkinnerToBaseModel();
                    break;
                case SkinFileSource.SkinBox:
                    SkinBoxRoot = Common.LoadJson<SkinBoxRoot>(filepath);
                    ConvertSkinBoxToBaseModel();
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
                    ConvertLSkinsToBaseModel();
                    break;
            }
        }

        public void SaveFile(string filepath, Config config, SkinFileSource skinFileSource = SkinFileSource.Skins)
        {
            switch (skinFileSource)
            {
                case SkinFileSource.Skins:
                    ConvertBaseModelToSkins();
                    Common.SaveJsonNewton<SkinsRoot>(SkinsRoot, filepath);
                    break;
                case SkinFileSource.SkinnerBeta:
                    try
                    {
                        FetchMissingSkinNames();
                    }
                    catch { }
                    ConvertBaseModelToSkinner();
                    Common.SaveJsonNewton<Dictionary<ulong, SkinnerRoot.SkinnerSkin>>(SkinnerRoot.Skins, filepath);
                    break;
            }
        }

        public async Task  SaveFiles(string folderpath, Config config, SkinFileSource skinFileSource = SkinFileSource.LSkins)
        {
            switch (skinFileSource)
            {
                case SkinFileSource.LSkins:
                    try
                    {
                        await FetchMissingSkinNames();
                    }
                    catch { }
                    ConvertBaseModelToLSkins();
                    //FetchSteamSkinsNamesLSkins();
                    foreach (var item in LSkinsRoot.Items)
                    {
                        Common.SaveJsonNewton<Dictionary<string, LSkinsRoot.LSkinItemSkin>>(item.Value.Skins, Path.Combine(folderpath, item.Key + ".json"));
                    }
                    //Common.SaveJsonNewton<SkinsRoot>(SkinsRoot, filepath);
                    break;
            }
        }

        public void ConverSkinsToBaseModel()
        {
            if (SkinsRoot != null)
            {
                BaseModel = new BaseModel();
                BaseModel.Items = new();

                foreach (var item in SkinsRoot.Skins)
                {
                    BaseItem baseItem = new BaseItem();
                    baseItem.Name = item.Name;
                    baseItem.Shortname = item.ItemShortname;
                    baseItem.Skins = new();

                    foreach (var skin in item.Skins)
                    {
                        if (skin == 0) continue; // Skip skins with WorkshopId 0
                        baseItem.Skins.Add(
                            new()
                            {
                                WorkshopId = skin,
                                Name = skin.ToString()
                            });
                    }

                    BaseModel.Items.Add(baseItem);
                }
            }
        }

        public void ConvertBaseModelToSkins()
        {
            if (BaseModel != null && BaseModel.Items != null)
            {
                SkinsRoot = new SkinsRoot();

                foreach (var item in BaseModel.Items)
                {
                    Skin skin = new Skin();
                    skin.ItemShortname = item.Shortname;
                    foreach (var baseItem in item.Skins)
                    {
                        if (baseItem.WorkshopId == 0) continue;

                        skin.Skins.Add(baseItem.WorkshopId);
                    }

                    SkinsRoot.Skins.Add(skin);
                }
            }
        }

        public void ConvertSkinnerToBaseModel()
        {
            if (SkinnerRoot != null && SkinnerRoot.Skins != null)
            {
                var results = SkinnerRoot.Skins.GroupBy(p => p.Value.itemShortname,
                    p => p, (key, g) => new { Shortname = key, Keys = g.ToList() });

                BaseModel = new BaseModel();
                BaseModel.Items = new();

                foreach (var item in results)
                {
                    BaseItem baseItem = new BaseItem();
                    baseItem.Shortname = item.Shortname;
                    baseItem.Skins = new();

                    foreach (var skin in item.Keys)
                    {
                        if (skin.Key == 0) continue; // Skip skins with WorkshopId 0
                        baseItem.Skins.Add(
                            new()
                            {
                                WorkshopId = skin.Key,
                                Name = skin.Value.itemDisplayname
                            });
                    }

                    BaseModel.Items.Add(baseItem);
                }
            }
        }

        public void ConvertBaseModelToSkinner()
        {
            if (BaseModel != null && BaseModel.Items != null)
            {
                SkinnerRoot = new SkinnerRoot();
                SkinnerRoot.Skins = new Dictionary<ulong, SkinnerRoot.SkinnerSkin>();

                foreach (var item in BaseModel.Items)
                {
                    foreach (var baseSkin in item.Skins)
                    {
                        if (baseSkin.WorkshopId == 0) continue;

                        if (!SkinnerRoot.Skins.ContainsKey(baseSkin.WorkshopId))
                        {
                            SkinnerRoot.Skins.Add(baseSkin.WorkshopId,
                                new SkinnerRoot.SkinnerSkin() { itemDisplayname = baseSkin.Name, itemShortname = item.Shortname });
                        }
                    }
                }
            }
        }

        public void ConvertSkinBoxToBaseModel()
        {
            if (SkinBoxRoot != null && SkinBoxRoot.Skins != null)
            {
                BaseModel = new BaseModel();
                BaseModel.Items = new();

                foreach (var item in SkinBoxRoot.Skins)
                {
                    BaseItem baseItem = new BaseItem();
                    baseItem.Shortname = item.Key;
                    baseItem.Skins = new();

                    foreach (var skin in item.Value)
                    {
                        if (skin == 0) continue; // Skip skins with WorkshopId 0
                        baseItem.Skins.Add(
                            new()
                            {
                                WorkshopId = skin,
                                Name = skin.ToString()
                            });
                    }

                    BaseModel.Items.Add(baseItem);
                }
            }
        }

        public void ConvertBaseModelToSkinBox()
        {
            if (BaseModel != null && BaseModel.Items != null)
            {
                SkinBoxRoot = new();
                SkinBoxRoot.Skins = new();

                foreach (var item in BaseModel.Items)
                {
                    SkinBoxRoot.Skins.Add(item.Shortname, item.Skins.Select(s => s.WorkshopId).ToList());
                }
            }
        }

        public void ConvertLSkinsToBaseModel()
        {
            if (LSkinsRoot != null && LSkinsRoot.Items != null)
            {
                BaseModel = new BaseModel();
                BaseModel.Items = new();

                foreach (var item in LSkinsRoot.Items)
                {
                    BaseItem baseItem = new BaseItem();
                    baseItem.Shortname = item.Key;
                    baseItem.Skins = new();

                    foreach (var lskin in item.Value.Skins)
                    {
                        if (lskin.Key == "0" || !UInt64.TryParse(lskin.Key, out ulong skinId)) continue; // Skip skins with WorkshopId 0
                        baseItem.Skins.Add(
                            new()
                            {
                                WorkshopId = skinId,
                                Name = lskin.Value.Name
                            });
                    }

                    BaseModel.Items.Add(baseItem);
                }
            }
        }

        public void ConvertBaseModelToLSkins()
        {
            if (BaseModel != null && BaseModel.Items != null)
            {
                LSkinsRoot = new();
                LSkinsRoot.Items = new();

                foreach (var item in BaseModel.Items)
                {
                    LSkinsRoot.LSkinItem lSkinItem = new LSkinsRoot.LSkinItem();
                    lSkinItem.Skins = new();

                    lSkinItem.Skins.Add("0", new()
                    {
                        Name = item.Name
                    });

                    foreach (var baseSkin in item.Skins)
                    {
                        lSkinItem.Skins.Add(baseSkin.WorkshopId.ToString(),
                            new()
                            {
                                Name = baseSkin.Name
                            });
                    }

                    LSkinsRoot.Items.Add(item.Shortname, lSkinItem);
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
            try
            {
                await FetchMissingSkinNames();
            }
            catch { }
            
            ConvertBaseModelToSkinner();

            if (SkinnerRoot == null) return;

            string skinnerJSON = Common.GetJsonString<SkinnerRoot>(SkinnerRoot);

            if (skinnerJSON != null && skinnerJSON.StartsWith('{') && skinnerJSON.EndsWith('}'))
            {
                skinnerJSON = Common.FormatJSON(skinnerJSON);
                skinnerJSON = skinnerJSON.Substring(1, skinnerJSON.Length - 2).Trim();

                SkinnerJSONString = skinnerJSON;
            }
        }

        public async Task FetchMissingSkinNames()
        {
            if (BaseModel == null) return;

            List<ulong> skinlist = new();

            foreach (var baseItem in BaseModel.Items)
            {
                skinlist.AddRange(baseItem.Skins.Where(s => string.IsNullOrEmpty(s.Name) 
                    || s.Name == s.WorkshopId.ToString()).Select(s => s.WorkshopId));
            }

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

                        bool found = false;
                        foreach (var baseItem in BaseModel.Items)
                        {
                            foreach (var baseSkin in baseItem.Skins)
                            {
                                if(baseSkin.WorkshopId == fileDetails.PublishedFileId)
                                {
                                    found = true;
                                    baseSkin.Name = fileDetails.Title;
                                    break;
                                }
                            }

                            if(found) break;
                        }
                    }
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
            ConvertBaseModelToSkinBox();

            if (SkinBoxRoot == null) return;


            string skinBoxJSON = Common.GetJsonString<SkinBoxRoot>(SkinBoxRoot);

            if (skinBoxJSON != null && skinBoxJSON.StartsWith('{') && skinBoxJSON.EndsWith('}'))
            {
                skinBoxJSON = Common.FormatJSON(skinBoxJSON);
                skinBoxJSON = skinBoxJSON.Substring(1, skinBoxJSON.Length - 2).Trim();

                SkinBoxJSONString = skinBoxJSON;
            }
        }
    }
}

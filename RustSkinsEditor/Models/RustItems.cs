using RustSkinsEditor.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Prism.Mvvm;
using System.Reflection;

namespace RustSkinsEditor.Models
{
    [DataContract]
    public class RustItems
    {
        public RustItems()
        {
            //Items = new List<RustItem>(); 
            //Load();
        }

        [DataMember]
        public List<RustItem> Items { get; set; }
        public DLCsData DLCsData { get; set; }

        public RustItem GetRustItem(string shortname)
        {
            return Items.FirstOrDefault(s => s.shortName == shortname);
        }

        public async Task Load(string steamPath)
        {
            await GetRustDLCs();
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string jsonPath = Path.Combine(appPath, "Assets", "items.json");

            List<RustItem> items = new();

            if (File.Exists(jsonPath))
                items = await Common.LoadJsonAsync<List<RustItem>>(jsonPath);

            //await FetchSkinnableItems(appPath, jsonPath, steamPath, items);

            Items = new List<RustItem>(items.OrderBy(x => x.displayName));
        }

        public void Load()
        {
            Items = Common.LoadJsonResource<List<RustItem>>("RustSkinsEditor.Assets.items.json");
        }

        public async Task<bool> GetRustDLCs()
        {
            DLCsData = new DLCsData();

            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dlcsPath = Path.Combine(appPath, "Assets", "dlcs.json");

            DateTime dt = File.GetLastWriteTime(dlcsPath);

            if ((DateTime.Now - dt).TotalMinutes > 30 || !File.Exists(dlcsPath))
            {
                var dlcs = await SteamApi.GetDLCs();

                if (dlcs != null && dlcs.meta.statusCode == 200 && dlcs.data != null && dlcs.data.Count > 0)
                {
                    DLCsData.Data = new List<RustDLCData>(dlcs.data);
                    Common.SaveJsonNewton(dlcs.data, dlcsPath, null, false);
                }
            }

            if (DLCsData.Data == null && File.Exists(dlcsPath))
                DLCsData.Data = await Common.LoadJsonAsync<List<RustDLCData>>(dlcsPath);

            if(DLCsData.Data != null)
            {
                foreach (var dlc in DLCsData.Data)
                {
                    if (dlc.workshopId.HasValue && dlc.workshopId != 0 && !DLCsData.ProhibitedSkins.Contains(dlc.workshopId.Value))
                    {
                        DLCsData.ProhibitedSkins.Add(dlc.workshopId.Value);
                    }
                }
            }


            //for testing
            //
            //List<string> notInFPList = new List<string>();
            //foreach (var carbonDLC in _carbonDLCShortnames)
            //{
            //    if (!DLCsData.DLCItems.Contains(carbonDLC))
            //    {
            //        notInFPList.Add(carbonDLC);
            //    }
            //}
            //List<string> notInCarbonList = new List<string>();
            //foreach (var fpDLC in DLCsData.DLCItems)
            //{
            //    if (!_carbonDLCShortnames.Contains(fpDLC))
            //    {
            //        notInCarbonList.Add(fpDLC);
            //    }
            //}
            //DLCsData.DLCItems = _carbonDLCShortnames;

            return true;
        }

        private async Task FetchSkinnableItems(string appPath, string jsonPath, string steamPath, List<RustItem> currentItems)
        {
            if (string.IsNullOrEmpty(steamPath)) return;

            string itemsDirectory = Path.Combine(steamPath, "steamapps\\common\\Rust\\Bundles\\items");

            if (!Directory.Exists(itemsDirectory)) return;

            var itemFiles = Directory.EnumerateFiles(itemsDirectory, "*.png");

            //List<string> skinnableItems = new List<string>();

            //foreach (var dlc in DLCsData.Data)
            //{
            //    if (dlc.workshopId.HasValue && dlc.workshopId != 0 && !DLCsData.ProhibitedSkins.Contains(dlc.workshopId.Value))
            //    {
            //        DLCsData.ProhibitedSkins.Add(dlc.workshopId.Value);

            //        if (!string.IsNullOrEmpty(dlc.itemShortName) && !skinnableItems.Contains(dlc.itemShortName))
            //            skinnableItems.Add(dlc.itemShortName);
            //    }
            //}

            bool newItemsFound = false;

            foreach (var item in itemFiles)
            {
                string shortname = Path.GetFileNameWithoutExtension(item);

                if (!currentItems.Any(s => s.shortName == shortname) && File.Exists(item.Replace(".png", ".json")))
                {
                    BundleItem bundleItem = await Common.LoadJsonAsync<BundleItem>(item.Replace(".png", ".json"));

                    if (string.IsNullOrEmpty(bundleItem.Name) || !skinnableItems.Contains(bundleItem.shortname)) continue;

                    await ResizeAndSaveImageFromSteam(appPath, shortname, steamPath);

                    currentItems.Add(new RustItem()
                    {
                        shortName = bundleItem.shortname,
                        category = bundleItem.Category,
                        displayName = bundleItem.Name
                    });

                    newItemsFound = true;
                }
            }

            if (newItemsFound)
            {
                Common.SaveJsonNewton(currentItems, jsonPath);
            }
        }

        private async Task ResizeAndSaveImageFromSteam(string appPath, string shortname, string steamPath)
        {

            string itempath = Path.Combine(appPath, "Assets", "RustItems", $"{shortname}.png");
            if (File.Exists(itempath)) return;

            string itemSteamPath = Path.Combine(steamPath, $"steamapps\\common\\Rust\\Bundles\\items\\{shortname}.png");
            if (!File.Exists(itemSteamPath)) return;

            await using FileStream fs = new FileStream(itemSteamPath, FileMode.Open);
            using Image source = new Bitmap(fs);
            using Image destination = new Bitmap(100, 100);

            using (var g = Graphics.FromImage(destination))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(source, new Rectangle(0, 0, destination.Width, destination.Height), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }
            destination.Save(itempath, ImageFormat.Png);
        }

        List<string> skinnableItems = new() {
            "attire.hide.boots", "attire.hide.helterneck", "attire.hide.pants", "attire.hide.poncho", "attire.hide.skirt", "attire.hide.vest", "barricade.concrete", "barricade.sandbags", "bone.club",
            "bow.hunting", "box.wooden", "box.wooden.large", "bucket.helmet", "burlap.gloves", "burlap.headwrap", "burlap.shirt", "burlap.shoes", "burlap.trousers",
            "chair", "coffeecan.helmet", "crossbow", "deer.skull.mask", "door.double.hinged.metal", "door.double.hinged.toptier", "door.double.hinged.wood", "door.hinged.metal", "door.hinged.toptier",
            "door.hinged.wood", "explosive.satchel", "fridge", "fun.guitar", "furnace", "grenade.f1", "hammer", "hat.beenie", "hat.boonie",
            "hat.cap", "hat.miner", "hoodie", "jacket", "knife.bone", "largebackpack", "longsleeve.tshirt", "mask.bandana", "metal.facemask",
            "metal.plate.torso", "pants", "rifle.ak", "rifle.bolt", "roadsign.gloves", "roadsign.jacket", "roadsign.kilt", "shirt.collared", "shirt.tanktop",
            "shoes.boots", "shorts", "spear.wooden", "sunglasses", "tshirt", "spinner.wheel", "beachchair", "beachparasol", "beachtowel", "boogieboard", "hazmatsuit",
            "twitch.headset", "innertube", "paddlingpool", "skullspikes", "skull.trophy", "sled", "gun.water"
        };
    }

    public class DLCsData
    {
        public List<RustDLCData> Data { get; set; }
        public List<ulong> ProhibitedSkins { get; set; } = new List<ulong>();
    }

    public class RustItem : BindableBase
    {
        private string _displayName;
        public string displayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        private string _shortName;
        public string shortName
        {
            get { return _shortName; }
            set { SetProperty(ref _shortName, value); }
        }

        private string _category;
        public string category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
    }
}

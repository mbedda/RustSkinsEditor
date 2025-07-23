using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RustSkinsEditor.Models
{
    public static class SteamModels
    {
        public static string GetShortnameFromWorkshopTags(List<string> workshopTags)
        {
            string shortname = "";

            foreach (string tag in workshopTags)
            {
                if (string.IsNullOrEmpty(tag))
                    continue;

                if (_workshopNameToShortname.ContainsKey(tag))
                {
                    shortname = _workshopNameToShortname[tag];
                    break;
                }
            }

            return shortname;
        }

        public static Dictionary<string, string> _workshopNameToShortname = new Dictionary<string, string>
        {
            {"Acoustic Guitar","fun.guitar"},
            {"AK47","rifle.ak"},
            {"Armored Double Door", "door.double.hinged.toptier"},
            {"Armored Door","door.hinged.toptier"},
            {"Large Backpack","largebackpack"},
            {"Balaclava","mask.balaclava"},
            {"Bandana","mask.bandana"},
            {"Bearskin Rug", "rug.bear"},
            {"Beenie Hat","hat.beenie"},
            {"Bolt Rifle","rifle.bolt"},
            {"Bone Club","bone.club"},
            {"Bone Knife","knife.bone"},
            {"Boonie Hat","hat.boonie"},
            {"Bucket Helmet","bucket.helmet"},
            {"Burlap Headwrap","burlap.headwrap"},
            {"Burlap Pants","burlap.trousers"},
            {"Burlap Shirt","burlap.shirt"},
            {"Burlap Shoes","burlap.shoes"},
            {"Cap","hat.cap"},
            {"Chair", "chair"},
            {"Coffee Can Helmet","coffeecan.helmet"},
            {"Collared Shirt","shirt.collared"},
            {"Combat Knife","knife.combat"},
            {"Concrete Barricade","barricade.concrete"},
            {"Crossbow","crossbow"},
            {"Custom SMG","smg.2"},
            {"Deer Skull Mask","deer.skull.mask"},
            {"Double Barrel Shotgun","shotgun.double"},
            {"Eoka Pistol","pistol.eoka"},
            {"F1 Grenade","grenade.f1"},
            {"Furnace","furnace"},
            {"Fridge", "fridge"},
            {"Garage Door", "wall.frame.garagedoor"},
            {"Hammer","hammer"},
            {"Hatchet","hatchet"},
            {"Hide Halterneck","attire.hide.helterneck"},
            {"Hide Pants","attire.hide.pants"},
            {"Hide Poncho","attire.hide.poncho"},
            {"Hide Shirt","attire.hide.vest"},
            {"Hide Shoes","attire.hide.boots"},
            {"Hide Skirt","attire.hide.skirt"},
            {"Hoodie","hoodie"},
            {"Hunting Bow","bow.hunting"},
            {"Jackhammer", "jackhammer"},
            {"Large Wood Box","box.wooden.large"},
            {"Leather Gloves","burlap.gloves"},
            {"Long TShirt","tshirt.long"},
            {"Longsword","longsword"},
            {"LR300","rifle.lr300"},
            {"Locker","locker"},
            {"L96", "rifle.l96"},
            {"Metal Chest Plate","metal.plate.torso"},
            {"Metal Facemask","metal.facemask"},
            {"Miner Hat","hat.miner"},
            {"Mp5","smg.mp5"},
            {"M39", "rifle.m39"},
            {"M249", "lmg.m249"},
            {"Pants","pants"},
            {"Pick Axe","pickaxe"},
            {"Pump Shotgun","shotgun.pump"},
            {"Python","pistol.python"},
            {"Reactive Target","target.reactive"},
            {"Revolver","pistol.revolver"},
            {"Riot Helmet","riot.helmet"},
            {"Roadsign Gloves", "roadsign.gloves"},
            {"Roadsign Pants","roadsign.kilt"},
            {"Roadsign Vest","roadsign.jacket"},
            {"Rock","rock"},
            {"Rocket Launcher","rocket.launcher"},
            {"Rug", "rug"},
            {"Rug Bear Skin","rug.bear"},
            {"Salvaged Hammer","hammer.salvaged"},
            {"Salvaged Icepick","icepick.salvaged"},
            {"Sandbag Barricade","barricade.sandbags"},
            {"Satchel Charge","explosive.satchel"},
            {"Semi-Automatic Pistol","pistol.semiauto"},
            {"Semi-Automatic Rifle","rifle.semiauto"},
            {"Sheet Metal Door","door.hinged.metal"},
            {"Sheet Metal Double Door","door.double.hinged.metal"},
            {"Shorts","pants.shorts"},
            {"Sleeping Bag","sleepingbag"},
            {"Snow Jacket","jacket.snow"},
            {"Stone Hatchet","stonehatchet"},
            {"Stone Pick Axe","stone.pickaxe"},
            {"Sword","salvaged.sword"},
            {"Table", "table"},
            {"Tank Top","shirt.tanktop"},
            {"Thompson","smg.thompson"},
            {"TShirt","tshirt"},
            {"Vagabond Jacket","jacket"},
            {"Vending Machine","vending.machine"},
            {"Water Purifier","water.purifier"},
            {"Waterpipe Shotgun","shotgun.waterpipe"},
            {"Wood Storage Box","box.wooden"},
            {"Wooden Door","door.hinged.wood"},
            {"Work Boots","shoes.boots"}
        };
    }

    [DataContract]
    public class SteamCollectionWebResponse
    {

        [DataMember(Name = "response")]
        public SteamResponse Response { get; set; }

        [DataContract]
        public class SteamResponse
        {
            [DataMember(Name = "result")]
            public int Result { get; set; }
            [DataMember(Name = "resultcount")]
            public int ResultCount { get; set; }
            [DataMember(Name = "collectiondetails")]
            public List<SteamCollection> CollectionDetails { get; set; }
        }

        [DataContract]
        public class SteamCollection
        {
            [DataMember(Name = "publishedfileid")]
            public string PublishedFileId { get; set; }
            [DataMember(Name = "result")]
            public int Result { get; set; }
            [DataMember(Name = "children")]
            public List<SteamFile> Children { get; set; }
        }

        [DataContract]
        public class SteamFile
        {
            [DataMember(Name = "publishedfileid")]
            public string PublishedFileId { get; set; }
            [DataMember(Name = "sortorder")]
            public int SortOrder { get; set; }
            [DataMember(Name = "filetype")]
            public int FileType { get; set; }
        }
    }

    [DataContract]
    public class RustDLCMeta
    {
        [DataMember(Name = "status")]
        public string status { get; set; }

        [DataMember(Name = "statusCode")]
        public int statusCode { get; set; }

    }

    [DataContract]
    public class RustDLCData
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "workshopId")]
        public ulong? workshopId { get; set; }

        [DataMember(Name = "itemDefinitionId")]
        public string? itemDefinitionId { get; set; }

        [DataMember(Name = "itemShortName")]
        public string itemShortName { get; set; }

    }

    [DataContract]
    public class RustDLCResponse
    {
        [DataMember(Name = "meta")]
        public RustDLCMeta meta { get; set; }

        [DataMember(Name = "data")]
        public IList<RustDLCData> data { get; set; }

    }

    //public class SteamSkinDetails
    //{
    //    public string shortname { get; set; }
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public ulong Code { get; set; }
    //    public Uri PreviewUrl { get; set; }
    //    public Uri WorkshopUrl { get; set; }
    //    public List<string> Tags { get; set; }
    //}
}

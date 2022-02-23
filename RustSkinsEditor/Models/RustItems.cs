using HtmlAgilityPack;
using Prism.Mvvm;
using RustSkinsEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RustSkinsEditor.Models
{
    [DataContract]
    public class RustItems
    {
        public RustItems()
        {
            Items = new List<RustItem>(); 
            Load();
        }

        [DataMember]
        public List<RustItem> Items { get; set; }

        //public RustItem MakeRustItem(string Name, string Shortname, string Thumbnail)
        //{
        //    return new RustItem(Name, Shortname, Thumbnail);
        //}

        public RustItem GetRustItem(string shortname)
        {
            return Items.FirstOrDefault(s => s.shortName == shortname);
        }

        //public void ParseData(List<string> shortnames)
        //{
        //    string url = "https://www.rust-items.com/";
        //    var web = new HtmlAgilityPack.HtmlWeb();
        //    HtmlDocument doc = web.Load(url);

        //    Items = new List<RustItem>();

        //    foreach (var shortname in shortnames)
        //    {
        //        HtmlNodeCollection htmlNode = doc.DocumentNode.SelectNodes("//*[text() = '"+ shortname + "']/../div[@class='itemIcon']/img");

        //        if(htmlNode == null)
        //            continue;

        //        string src = htmlNode.First().Attributes["src"].Value;
        //        string alt = htmlNode.First().Attributes["alt"].Value;

        //        Items.Add(MakeRustItem(alt, shortname, src));
        //    }
        //    Save();
        //}

        public void Load()
        {
            Items = Common.LoadJsonResource<List<RustItem>>("RustSkinsEditor.Assets.items.json");
        }

        //public void Save()
        //{
        //    Common.SaveJson(Items, "C:\\Users\\mbedd\\Downloads\\2022-02\\rustitems.json");
        //}
    }
    [DataContract]
    public class RustItem
    {
        public RustItem()
        {
            image = new Uri("https://www.rustedit.io/images/imagelibrary/" + shortName + ".png");
        }

        [DataMember]
        public string id { get; set; }
        [DataMember]
        public Uri image { get; set; }
        [DataMember]
        public string displayName { get; set; }
        [DataMember]
        public string shortName { get; set; }
        [DataMember]
        public string category { get; set; }
    }
}

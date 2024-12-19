using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustSkinsEditor.Models
{
    public class BundleItem
    {
        public int itemid { get; set; }
        public string shortname { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int maxDraggable { get; set; }
        public string ItemType { get; set; }
        public string AmountType { get; set; }
        public int stackable { get; set; }
        public bool quickDespawn { get; set; }
        public string rarity { get; set; }
        public Condition condition { get; set; }
        public int Parent { get; set; }
        public bool isWearable { get; set; }
        public bool isHoldable { get; set; }
        public bool isUsable { get; set; }
        public bool HasSkins { get; set; }
    }
    public class Condition
    {
        public bool enabled { get; set; }
        public double max { get; set; }
        public bool repairable { get; set; }
    }
}

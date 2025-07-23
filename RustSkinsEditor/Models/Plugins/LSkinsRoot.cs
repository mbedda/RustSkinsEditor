using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RustSkinsEditor.Models.Plugins
{
    public class LSkinsRoot
    {
        public Dictionary<string, LSkinItem> Items { get; set; } = new ();

        public class LSkinItem
        {
            public Dictionary<string, LSkinItemSkin> Skins { get; set; } = new();
        }

        [DataContract]
        public class LSkinItemSkin
        {
            [DataMember(Name = "Enabled skin?(true = yes)")]
            public bool Enabled { get; set; } = true;

            [DataMember(Name = "Is this skin from the developers of rust or take it in a workshop?")]
            public bool FromFP { get; set; } = true;

            [DataMember(Name = "Is item")]
            public bool IsItem { get; set; } = false;

            [DataMember(Name = "Shortname, если предмет")]
            public string? Shortname { get; set; } = null;

            [DataMember(Name = "Name skin")]
            public string Name { get; set; }
        }
    }
}

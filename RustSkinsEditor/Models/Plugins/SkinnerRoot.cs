using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RustSkinsEditor.Models.Plugins
{
    [DataContract]
    public class SkinnerRoot
    {
        [DataMember(Name = "Imported Skins List")]
        public Dictionary<ulong, SkinnerSkin> Skins { get; set; }

        [DataContract]
        public class SkinnerSkin
        {
            [DataMember(Name = "itemShortname", Order = 0)]
            public string itemShortname { get; set; }
            [DataMember(Name = "itemDisplayname", Order = 1)]
            public string itemDisplayname { get; set; }
        }
    }
}

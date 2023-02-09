using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
            [DataMember(Name = "itemShortname")]
            public string itemShortname { get; set; }
            [DataMember(Name = "itemDisplayname")]
            public string itemDisplayname { get; set; }
        }
    }
}

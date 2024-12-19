using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RustSkinsEditor.Models.Plugins
{
    [DataContract]
    public class SkinBoxRoot
    {
        [DataMember(Name = "Imported Workshop Skins")]
        public Dictionary<string, List<ulong>> Skins { get; set; }
    }
}

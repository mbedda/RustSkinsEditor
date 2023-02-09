using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RustSkinsEditor.Models.Plugins
{
    [DataContract]
    public class SkinBoxRoot
    {
        [DataMember(Name = "Imported Workshop Skins")]
        public Dictionary<string, List<ulong>> Skins { get; set; }
    }
}

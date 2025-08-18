using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RustSkinsEditor.Models.Plugins
{
    public class SkinControllerRoot
    {
        public Dictionary<int, List<SkinControllerSkin>> Items { get; set; }

        [DataContract]
        public class SkinControllerSkin
        {
            [DataMember(Order = 0)]
            public SkinControllerRedirect Redirect { get; set; } = new ();

            [DataMember(Order = 1)]
            public string Category { get; set; }

            [DataMember(Order = 2)]
            public int ItemID { get; set; }

            [DataMember(Order = 3)]
            public string ItemShortname { get; set; }

            [DataMember(Order = 4)]
            public ulong SkinID { get; set; }

            [DataMember(Order = 5)]
            public string SkinName { get; set; }

            [DataMember(Order = 6)]
            public string SkinPermission { get; set; } = "";
        }

        [DataContract]
        public class SkinControllerRedirect
        {
            [DataMember(Order = 0)]
            public bool IsRedirect { get; set; } = false;

            [DataMember(Order = 1)]
            public int ItemId { get; set; } = 0;

            [DataMember(Order = 2)]
            public string Shortname { get; set; } = "";
        }
    }
}

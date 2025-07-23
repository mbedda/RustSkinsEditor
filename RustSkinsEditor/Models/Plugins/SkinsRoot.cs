using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace RustSkinsEditor.Models.Plugins
{
    [DataContract]
    public class SkinsRoot
    {
        public SkinsRoot()
        {
            Skins = new ObservableCollection<Skin>();
        }

        [DataMember]
        public List<string> Commands { get; set; } = new List<string>() { "skin", "skins" };
        [DataMember]
        public ObservableCollection<Skin> Skins { get; set; }

        [DataMember(Name = "Container Panel Name")]
        public string ContainerPanelName { get; set; } = "generic";

        [DataMember(Name = "Container Capacity")]
        public int ContainerCapacity { get; set; } = 36;
        [DataMember(Name = "UI")]
        public UI Ui { get; set; } = new UI();



        [DataContract]
        public class BackgroundAnchors
        {
            [DataMember(Name = "Anchor Min X")]
            public string AnchorMinX { get; set; }

            [DataMember(Name = "Anchor Min Y")]
            public string AnchorMinY { get; set; }

            [DataMember(Name = "Anchor Max X")]
            public string AnchorMaxX { get; set; }

            [DataMember(Name = "Anchor Max Y")]
            public string AnchorMaxY { get; set; }
        }

        [DataContract]
        public class BackgroundOffsets
        {
            [DataMember(Name = "Offset Min X")]
            public string OffsetMinX { get; set; }

            [DataMember(Name = "Offset Min Y")]
            public string OffsetMinY { get; set; }

            [DataMember(Name = "Offset Max X")]
            public string OffsetMaxX { get; set; }

            [DataMember(Name = "Offset Max Y")]
            public string OffsetMaxY { get; set; }
        }

        [DataContract]
        public class LeftButtonAnchors
        {
            [DataMember(Name = "Anchor Min X")]
            public string AnchorMinX { get; set; }

            [DataMember(Name = "Anchor Min Y")]
            public string AnchorMinY { get; set; }

            [DataMember(Name = "Anchor Max X")]
            public string AnchorMaxX { get; set; }

            [DataMember(Name = "Anchor Max Y")]
            public string AnchorMaxY { get; set; }
        }

        [DataContract]
        public class CenterButtonAnchors
        {
            [DataMember(Name = "Anchor Min X")]
            public string AnchorMinX { get; set; }

            [DataMember(Name = "Anchor Min Y")]
            public string AnchorMinY { get; set; }

            [DataMember(Name = "Anchor Max X")]
            public string AnchorMaxX { get; set; }

            [DataMember(Name = "Anchor Max Y")]
            public string AnchorMaxY { get; set; }
        }

        [DataContract]
        public class RightButtonAnchors
        {
            [DataMember(Name = "Anchor Min X")]
            public string AnchorMinX { get; set; }

            [DataMember(Name = "Anchor Min Y")]
            public string AnchorMinY { get; set; }

            [DataMember(Name = "Anchor Max X")]
            public string AnchorMaxX { get; set; }

            [DataMember(Name = "Anchor Max Y")]
            public string AnchorMaxY { get; set; }
        }

        [DataContract]
        public class UI
        {
            [DataMember(Name = "Background Color")]
            public string BackgroundColor { get; set; } = "0.18 0.28 0.36";

            [DataMember(Name = "Background Anchors")]
            public BackgroundAnchors BackgroundAnchors { get; set; } = new BackgroundAnchors() { AnchorMinX = "1.0", AnchorMinY = "1.0", AnchorMaxX = "1.0", AnchorMaxY = "1.0" };

            [DataMember(Name = "Background Offsets")]
            public BackgroundOffsets BackgroundOffsets { get; set; } = new BackgroundOffsets() { OffsetMinX = "-300", OffsetMinY = "-100", OffsetMaxX = "0", OffsetMaxY = "0" };

            [DataMember(Name = "Left Button Text")]
            public string LeftButtonText { get; set; } = "<size=36><</size>";

            [DataMember(Name = "Left Button Color")]
            public string LeftButtonColor { get; set; } = "0.11 0.51 0.83";

            [DataMember(Name = "Left Button Anchors")]
            public LeftButtonAnchors LeftButtonAnchors { get; set; } = new LeftButtonAnchors() { AnchorMinX = "0.025", AnchorMinY = "0.05", AnchorMaxX = "0.325", AnchorMaxY = "0.95" };

            [DataMember(Name = "Center Button Text")]
            public string CenterButtonText { get; set; } = "<size=36>Page: {page}</size>";

            [DataMember(Name = "Center Button Color")]
            public string CenterButtonColor { get; set; } = "0.11 0.51 0.83";

            [DataMember(Name = "Center Button Anchors")]
            public CenterButtonAnchors CenterButtonAnchors { get; set; } = new CenterButtonAnchors() { AnchorMinX = "0.350", AnchorMinY = "0.05", AnchorMaxX = "0.650", AnchorMaxY = "0.95" };

            [DataMember(Name = "Right Button Text")]
            public string RightButtonText { get; set; } = "<size=36>></size>";

            [DataMember(Name = "Right Button Color")]
            public string RightButtonColor { get; set; } = "0.11 0.51 0.83";

            [DataMember(Name = "Right Button Anchors")]
            public RightButtonAnchors RightButtonAnchors { get; set; } = new RightButtonAnchors() { AnchorMinX = "0.675", AnchorMinY = "0.05", AnchorMaxX = "0.975", AnchorMaxY = "0.95" };
        }
    }

    [DataContract]
    public class Skin
    {
        public Skin()
        {
            Skins = new List<ulong>();
        }

        [IgnoreDataMember]
        public string Name { get; set; }
        [DataMember(Name = "Item Shortname")]
        public string ItemShortname { get; set; }
        [DataMember(Name = "Permission")]
        public string Permission { get; set; }
        [DataMember(Name = "Skins")]
        public List<ulong> Skins { get; set; }
    }
}

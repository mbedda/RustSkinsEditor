using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RustSkinsEditor.Models
{
    public class SkinDetails : BindableBase
    {
        public string shortname { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ulong Code { get; set; }
        public Uri PreviewUrl { get; set; }
        public Uri WorkshopUrl { get; set; }
    }

    public class SkinsFile : BindableBase
    {
        private SkinsRoot skinsRoot;
        public SkinsRoot SkinsRoot
        {
            get { return skinsRoot; }
            set { SetProperty(ref skinsRoot, value); }
        }
    }

    [DataContract]
    public class SkinsRoot
    {
        [DataMember]
        public List<string> Commands { get; set; }
        [DataMember]
        public List<Skin> Skins { get; set; }

        [DataMember(Name = "Container Panel Name")]
        public string ContainerPanelName { get; set; }

        [DataMember(Name = "Container Capacity")]
        public int ContainerCapacity { get; set; }
        [DataMember]
        public UI UI { get; set; }
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
        [IgnoreDataMember]
        public Uri ImagePath { get; set; }
        [DataMember(Name = "Item Shortname")]
        public string ItemShortname { get; set; }
        [DataMember(Name = "Permission")]
        public string Permission { get; set; }
        [DataMember(Name = "Skins")]
        public List<ulong> Skins { get; set; }
    }

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
        public string BackgroundColor { get; set; }

        [DataMember(Name = "Background Anchors")]
        public BackgroundAnchors BackgroundAnchors { get; set; }

        [DataMember(Name = "Background Offsets")]
        public BackgroundOffsets BackgroundOffsets { get; set; }

        [DataMember(Name = "Left Button Text")]
        public string LeftButtonText { get; set; }

        [DataMember(Name = "Left Button Color")]
        public string LeftButtonColor { get; set; }

        [DataMember(Name = "Left Button Anchors")]
        public LeftButtonAnchors LeftButtonAnchors { get; set; }

        [DataMember(Name = "Center Button Text")]
        public string CenterButtonText { get; set; }

        [DataMember(Name = "Center Button Color")]
        public string CenterButtonColor { get; set; }

        [DataMember(Name = "Center Button Anchors")]
        public CenterButtonAnchors CenterButtonAnchors { get; set; }

        [DataMember(Name = "Right Button Text")]
        public string RightButtonText { get; set; }

        [DataMember(Name = "Right Button Color")]
        public string RightButtonColor { get; set; }

        [DataMember(Name = "Right Button Anchors")]
        public RightButtonAnchors RightButtonAnchors { get; set; }
    }
}

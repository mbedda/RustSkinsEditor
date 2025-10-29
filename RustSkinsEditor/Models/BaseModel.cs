using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace RustSkinsEditor.Models
{
    public class BaseModel : BindableBase
    {
        private ObservableCollection<BaseItem> _items;
        public ObservableCollection<BaseItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
    }

    public class BaseItem : BindableBase
    {
        private string _shortname;
        public string Shortname
        {
            get { return _shortname; }
            set { SetProperty(ref _shortname, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private ObservableCollection<BaseSkin> _skins = new ObservableCollection<BaseSkin>();
        public ObservableCollection<BaseSkin> Skins
        {
            get { return _skins; }
            set { SetProperty(ref _skins, value); }
        }
    }

    public class BaseSkin : BindableBase
    {
        private ulong _workshopId;
        public ulong WorkshopId
        {
            get { return _workshopId; }
            set { SetProperty(ref _workshopId, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private Uri _previewUrl;
        public Uri PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }

        private Uri _workshopUrl;
        public Uri WorkshopUrl
        {
            get { return _workshopUrl; }
            set { SetProperty(ref _workshopUrl, value); }
        }

        private bool _steamDataFetched;
        public bool SteamDataFetched
        {
            get { return _steamDataFetched; }
            set { SetProperty(ref _steamDataFetched, value); }
        }

        private bool _invalid;
        public bool Invalid
        {
            get { return _invalid; }
            set { SetProperty(ref _invalid, value); }
        }
    }
}

using System;
using System.ComponentModel;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaFile : IMediaFile
    {
        public MediaFile()
        {
        }

        public MediaFile(string url, MediaFileType type)
        {
            Url = url;
            Type = type;
        }

        public Guid Id { get; set; }

        public object Cover { get; set; }

        public MediaFileType Type { get; set; }

        public string Url { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
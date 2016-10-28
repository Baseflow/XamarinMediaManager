using System;
using System.ComponentModel;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaFile : IMediaFile
    {
        public MediaFile() : this(String.Empty, MediaFileType.Other)
        {
        }

        public MediaFile(string url, MediaFileType type)
        {
            Url = url;
            Type = type;
            Metadata = new MediaFileMetadata();
        }

        public Guid Id { get; set; }
        public MediaFileType Type { get; set; }
        public IMediaFileMetadata Metadata { get; set; }

        public string Url { get; set; }
        public string Title { get; set; }
        public bool MetadataExtracted { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
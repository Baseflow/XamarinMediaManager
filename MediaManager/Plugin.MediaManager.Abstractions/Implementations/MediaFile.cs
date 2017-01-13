using System;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

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
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public MediaFileType Type { get; set; }

        public IMediaFileMetadata Metadata { get; set; } = new MediaFileMetadata();

        public string Url { get; set; }

        private bool _metadataExtracted;
        public bool MetadataExtracted { get {
                return _metadataExtracted;
            } set {
                _metadataExtracted = value;
                MetadataUpdated?.Invoke(this, new MetadataChangedEventArgs(Metadata));
            } }

        public event MetadataUpdatedEventHandler MetadataUpdated;
    }
}
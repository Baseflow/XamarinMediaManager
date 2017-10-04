using System;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    public class MediaItem : IMediaItem
    {
        public MediaItem() : this(String.Empty, default(MediaItemType))
        {
        }

        public MediaItem(string url) : this(url, default(MediaItemType))
        {
        }

        public MediaItem(string url, MediaItemType type)
        {
            Url = url;
            Type = type;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public MediaItemType Type { get; set; }

        public IMediaItemMetadata Metadata { get; set; } = new MediaItemMetadata();

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
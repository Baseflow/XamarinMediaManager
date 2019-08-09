using System;
using MediaManager.Library;

namespace MediaManager.Media
{
    public class MetadataChangedEventArgs : EventArgs
    {
        public MetadataChangedEventArgs(IMediaItem mediaItem)
        {
            MediaItem = mediaItem;
        }

        public IMediaItem MediaItem { get; set; }
    }
}

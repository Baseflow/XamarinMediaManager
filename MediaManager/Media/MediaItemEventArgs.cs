using System;
using MediaManager.Media;

namespace MediaManager.Media
{
    public class MediaItemEventArgs : EventArgs
    {
        public MediaItemEventArgs(IMediaItem mediaItem)
        {
            MediaItem = mediaItem;
        }

        public IMediaItem MediaItem { get; private set; }
    }
}

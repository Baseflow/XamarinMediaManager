using System;
using MediaManager.Media;

namespace MediaManager.Media
{
    public class MediaItemEventArgs : EventArgs
    {
        public MediaItemEventArgs(IMediaItem Item)
        {
            this.Item = Item;
        }

        public IMediaItem Item { get; private set; }
    }
}

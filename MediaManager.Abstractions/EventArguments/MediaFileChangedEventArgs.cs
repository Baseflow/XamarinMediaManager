using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaItemChangedEventArgs : EventArgs
    {
        public MediaItemChangedEventArgs(IMediaItem item)
        {
            Item = item;
        }

        public IMediaItem Item { get; set; }
    }
}
using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFileChangedEventArgs : EventArgs
    {
        public MediaFileChangedEventArgs(IMediaItem item)
        {
            Item = item;
        }

        public IMediaItem Item { get; set; }
    }
}
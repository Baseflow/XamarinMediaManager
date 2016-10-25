using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFileChangedEventArgs : EventArgs
    {
        public MediaFileChangedEventArgs(IMediaFile file)
        {
            File = file;
        }

        public IMediaFile File { get; set; }
    }
}
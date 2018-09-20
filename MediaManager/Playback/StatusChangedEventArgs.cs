using System;

namespace MediaManager.Playback
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(MediaPlayerState status)
        {
            Status = status;
        }

        public MediaPlayerState Status { get; }
    }
}

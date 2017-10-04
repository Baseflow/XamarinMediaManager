using System;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(PlaybackState state)
        {
            State = state;
        }
        public PlaybackState State { get; }
    }
}
using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class VolumeChangedEventArgs : EventArgs
    {
        public VolumeChangedEventArgs(object volumeArguments, bool mute)
        {
            Volume = volumeArguments;
            Mute = mute;
        }

        public object Volume { get; }
        public bool Mute { get; }
    }
}
using System;

namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class VolumeChangedEventArgs : EventArgs
    {
        public VolumeChangedEventArgs(object volumeArguments)
        {
            Volume = volumeArguments;
        }

        public object Volume { get; set; }
    }
}
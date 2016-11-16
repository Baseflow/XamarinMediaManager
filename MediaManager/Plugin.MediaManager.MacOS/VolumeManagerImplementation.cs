using System;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : IVolumeManager
    {
        public int CurrentVolume { get; set; }

        public int MaxVolume { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;

        public bool Mute { get; set; }
    }
}

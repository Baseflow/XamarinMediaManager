using System;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : IVolumeManager
    {
        public float CurrentVolume { get; set; }

        public float MaxVolume { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;

        public bool Mute { get; set; }
    }
}

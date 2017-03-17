using System;
using Android.Support.V4.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : VolumeProviderCompat.Callback, IVolumeManager
    {
        public float CurrentVolume { get; set; }

        public float MaxVolume { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;

        public override void OnVolumeChanged(VolumeProviderCompat volumeProvider)
        {
            VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volumeProvider, Mute));
        }

        public bool Mute { get; set; }
    }
}

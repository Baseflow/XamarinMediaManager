using System;
using Android.Support.V4.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : VolumeProviderCompat.Callback, IVolumeManager
    {
        private MediaManagerImplementation _mediaManagerImplementation;

        public VolumeManagerImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            _mediaManagerImplementation = mediaManagerImplementation;
        }

        public int CurrentVolume { get; set; }

        public int MaxVolume { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;

        public override void OnVolumeChanged(VolumeProviderCompat volumeProvider)
        {
            VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volumeProvider.CurrentVolume, Muted));
        }

        public bool Muted { get; set; }
    }
}

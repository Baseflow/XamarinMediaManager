using System;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : IVolumeManager
    {
        private int currentVolume = 0;
        public int CurrentVolume
        {
            get { return currentVolume; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MaxVolume)
                    value = MaxVolume;
                else
                    currentVolume = value;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, muted));
            }
        }

        private int maxVolume = 0;
        public int MaxVolume
        {
            get => maxVolume;
            set
            {
                maxVolume = value;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, muted));
            }
        }

        public event VolumeChangedEventHandler VolumeChanged;

        private bool muted = false;
        public bool Muted
        {
            get => muted;
            set
            {
                muted = value;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(currentVolume, muted));
            }
        }
    }
}

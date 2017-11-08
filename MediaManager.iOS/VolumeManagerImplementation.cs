using System;
using AVFoundation;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : IVolumeManager
    {
        internal AVPlayer player;

        public VolumeManagerImplementation()
        {
        }

        public int CurrentVolume
        {
            get
            {
                int.TryParse((player.Volume * 100).ToString(), out var vol);
                return vol;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MaxVolume)
                    value = MaxVolume;

                float vol = value;
                if (value > 0) float.TryParse((value / 100).ToString(), out vol);
                player.Volume = vol;

                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        private int maxVolume = 100;
        public int MaxVolume
        {
            get => maxVolume;
            set
            {
                maxVolume = value;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        public event VolumeChangedEventHandler VolumeChanged;

        public bool Muted
        {
            get => player?.Muted ?? false;
            set
            {
                player.Muted = value;
                int.TryParse((player.Volume * 100).ToString(), out var vol);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(vol, value));
            }
        }
    }
}

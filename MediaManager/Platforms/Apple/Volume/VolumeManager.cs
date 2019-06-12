using AVFoundation;
using MediaManager.Volume;

namespace MediaManager.Platforms.Apple
{
    public class VolumeManager : IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Apple;
        protected AVQueuePlayer Player => MediaManager.AppleMediaPlayer.Player;

        public VolumeManager()
        {
        }

        public int CurrentVolume
        {
            get
            {
                int.TryParse((Player.Volume * 100).ToString(), out var vol);
                return vol;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MaxVolume)
                    value = MaxVolume;

                float vol = value;
                if (value > 0) float.TryParse((value / 100.0).ToString(), out vol);
                Player.Volume = vol;

                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        private int _maxVolume = 100;
        public int MaxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        public event VolumeChangedEventHandler VolumeChanged;

        public bool Muted
        {
            get => Player?.Muted ?? false;
            set
            {
                Player.Muted = value;
                int.TryParse((Player.Volume * 100).ToString(), out var vol);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(vol, value));
            }
        }
    }
}

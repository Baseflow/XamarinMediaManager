using AVFoundation;
using MediaManager.Volume;

namespace MediaManager.Platforms.Apple
{
    public class VolumeManager : IVolumeManager
    {
        protected AVPlayer _player;

        public VolumeManager(AVPlayer player)
        {
            _player = player;
        }

        public int CurrentVolume
        {
            get
            {
                int.TryParse((_player.Volume * 100).ToString(), out var vol);
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
                _player.Volume = vol;

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
            get => _player?.Muted ?? false;
            set
            {
                _player.Muted = value;
                int.TryParse((_player.Volume * 100).ToString(), out var vol);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(vol, value));
            }
        }
    }
}

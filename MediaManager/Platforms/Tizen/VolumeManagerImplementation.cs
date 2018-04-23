using Tizen.Multimedia;
using Plugin.MediaManager.Abstractions;
using EA = Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VolumeManagerImplementation : IVolumeManager
    {
        internal Player _player;
        private int _maxVolume = 100;
        private int _currentVolume = 50;

        public VolumeManagerImplementation()
        {
        }

        public int CurrentVolume
        {
            get => _currentVolume;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MaxVolume)
                    value = MaxVolume;
                _currentVolume = value;
                if (_player != null)
                {
                    float vol = value;
                    if (value > 0) float.TryParse((value / 100.0).ToString(), out vol);
                    _player.Volume = vol;
                }
                VolumeChanged?.Invoke(this, new EA.VolumeChangedEventArgs(value, Muted));
            }
        }

        public int MaxVolume
        {
            get => _maxVolume;
            set
            {
                if (value == MaxVolume)
                    return;
                _maxVolume = value;
                VolumeChanged?.Invoke(this, new EA.VolumeChangedEventArgs(value, Muted));
            }
        }

        public bool Muted
        {
            get
            {
                if (_player == null || _player.State == PlayerState.Preparing)
                    return false;
                return _player.Muted;
            }
            set
            {                
                if (_player == null || Muted == value || _player.State == PlayerState.Preparing)
                    return;
                _player.Muted = value;
                VolumeChanged?.Invoke(this, new EA.VolumeChangedEventArgs(CurrentVolume, value));
            }
        }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}

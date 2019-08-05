using Windows.Media.Playback;
using MediaManager.Volume;

namespace MediaManager.Platforms.Uap.Volume
{
    public class VolumeManager : IVolumeManager
    {
        private const int VolumeRangeMax = 100;
        private const int VolumeRangeMin = 0;

        protected MediaManagerImplementation MediaManager => CrossMediaManager.Windows;
        private MediaPlayer MediaPlayer => MediaManager.WindowsMediaPlayer.Player;

        public int CurrentVolume
        {
            get => FromNative(MediaPlayer.Volume);
            set
            {
                int volume = value;
                volume = Clamp(volume, VolumeRangeMin, MaxVolume);
                MediaPlayer.Volume = ToNative(volume);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volume, Muted));
            }
        }

        private int _maxVolume = VolumeRangeMax;
        public int MaxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value;
                if (CurrentVolume > _maxVolume)
                    CurrentVolume = _maxVolume;
            }
        }

        public bool Muted
        {
            get => MediaPlayer?.IsMuted ?? false;
            set
            {
                MediaPlayer.IsMuted = value;
                var volume = FromNative(MediaPlayer.Volume);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volume, value));
            }
        }

        public float Balance { get; set; }

        public event VolumeChangedEventHandler VolumeChanged;

        private int FromNative(double nativeVolume)
        {
            return (int)(nativeVolume * VolumeRangeMax);
        }

        private double ToNative(int volume)
        {
            return volume / (double)VolumeRangeMax;
        }

        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}

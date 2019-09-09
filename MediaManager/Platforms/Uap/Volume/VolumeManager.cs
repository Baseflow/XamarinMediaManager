using MediaManager.Volume;
using Windows.Media.Playback;

namespace MediaManager.Platforms.Uap.Volume
{
    public class VolumeManager : VolumeManagerBase, IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Windows;
        protected MediaPlayer MediaPlayer => MediaManager.WindowsMediaPlayer.Player;

        protected const int VolumeRangeMax = 100;
        protected const int VolumeRangeMin = 0;

        public VolumeManager()
        {
            MediaManager.Player.VolumeChanged += Player_VolumeChanged;
            MediaManager.Player.IsMutedChanged += Player_IsMutedChanged;
        }

        public override int CurrentVolume
        {
            get => FromNative(MediaPlayer.Volume);
            set
            {
                var volume = value;
                volume = Clamp(volume, VolumeRangeMin, MaxVolume);
                MediaPlayer.Volume = ToNative(volume);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volume, Muted));
            }
        }

        private int _maxVolume = VolumeRangeMax;
        public override int MaxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value;
                if (CurrentVolume > _maxVolume)
                    CurrentVolume = _maxVolume;
            }
        }

        public override bool Muted
        {
            get => MediaPlayer?.IsMuted ?? false;
            set
            {
                MediaPlayer.IsMuted = value;
                var volume = FromNative(MediaPlayer.Volume);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(volume, value));
            }
        }

        public override float Balance { get; set; }

        public override event VolumeChangedEventHandler VolumeChanged;

        private void Player_IsMutedChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            //Muted = sender.IsMuted;
        }

        private void Player_VolumeChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            //CurrentVolume = Convert.ToInt32(sender.Volume);
        }

        protected int FromNative(double nativeVolume)
        {
            return (int)(nativeVolume * VolumeRangeMax);
        }

        protected double ToNative(int volume)
        {
            return volume / (double)VolumeRangeMax;
        }

        protected int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}

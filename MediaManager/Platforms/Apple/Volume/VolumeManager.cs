using AVFoundation;
using MediaManager.Volume;

namespace MediaManager.Platforms.Apple.Volume
{
    public class VolumeManager : VolumeManagerBase, IVolumeManager
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Apple;
        protected AVQueuePlayer Player => MediaManager.Player;

        public VolumeManager()
        {
        }

        public override event VolumeChangedEventHandler VolumeChanged;

        public override int CurrentVolume
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
        public override int MaxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        public override float Balance { get; set; }

        public override bool Muted
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

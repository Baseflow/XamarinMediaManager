using Android.Support.V4.Media.Session;
using MediaManager.Volume;

namespace MediaManager.Platforms.Android.Volume
{
    public class VolumeManager : VolumeManagerBase, IVolumeManager //VolumeProviderCompat.Callback
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected MediaControllerCompat MediaController => MediaManager.MediaController;

        public VolumeManager()
        {
        }

        public override event VolumeChangedEventHandler VolumeChanged;

        public override int CurrentVolume
        {
            get => MediaController.GetPlaybackInfo().CurrentVolume;
            set
            {
                MediaController.SetVolumeTo(value, 0);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        public override int MaxVolume
        {
            get => MediaController.GetPlaybackInfo().MaxVolume;
            set
            {
                if (CurrentVolume > value)
                {
                    CurrentVolume = value;
                    VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(CurrentVolume, Muted));
                }
            }
        }

        protected int preMutedVolume = 0;
        public override bool Muted
        {
            get => CurrentVolume == 0;
            set
            {
                if (!Muted)
                {
                    preMutedVolume = CurrentVolume;
                    CurrentVolume = 0;
                }
                else
                    CurrentVolume = preMutedVolume;

                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(CurrentVolume, Muted));
            }
        }

        public override float Balance { get; set; }
    }
}

using Android.Support.V4.Media.Session;
using MediaManager.Volume;

namespace MediaManager.Platforms.Android
{
    public class VolumeManager : IVolumeManager //VolumeProviderCompat.Callback
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected MediaControllerCompat MediaController => MediaManager.MediaBrowserManager.MediaController;

        public VolumeManager()
        {
        }

        public int CurrentVolume
        {
            get => MediaController.GetPlaybackInfo().CurrentVolume;
            set
            {
                MediaController.SetVolumeTo(value, 0);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        public int MaxVolume
        {
            get => MediaController.GetPlaybackInfo().MaxVolume;
            set
            {
                if (CurrentVolume > value)
                    CurrentVolume = value;
            }
        }

        protected int preMutedVolume = 0;
        public bool Muted
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
            }
        }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}

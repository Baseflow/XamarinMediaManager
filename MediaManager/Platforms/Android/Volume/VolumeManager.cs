using Android.Support.V4.Media.Session;
using MediaManager.Volume;

namespace MediaManager.Platforms.Android
{
    public class VolumeManager : IVolumeManager //VolumeProviderCompat.Callback
    {
        private MediaManagerImplementation _mediaManagerImplementation;
        private MediaControllerCompat _mediaController => _mediaManagerImplementation.MediaBrowserManager.MediaController;

        //TODO: Probably inject another class
        public VolumeManager(MediaManagerImplementation mediaManagerImplementation)
        {
            _mediaManagerImplementation = mediaManagerImplementation;
        }

        public int CurrentVolume
        {
            get => _mediaController.GetPlaybackInfo().CurrentVolume;
            set
            {
                _mediaController.SetVolumeTo(value, 0);
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(value, Muted));
            }
        }

        public int MaxVolume
        {
            get => _mediaController.GetPlaybackInfo().MaxVolume;
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

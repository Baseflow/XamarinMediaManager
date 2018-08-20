using Android.Media;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using MediaManager.Volume;

namespace MediaManager
{
    public class VolumeManager : IVolumeManager //VolumeProviderCompat.Callback
    {
        private MediaManagerImplementation mediaManagerImplementation;
        private MediaControllerCompat mediaController => mediaManagerImplementation.MediaBrowserManager.mediaController;

        public VolumeManager(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }
        
        public int CurrentVolume { get => mediaController.GetPlaybackInfo().CurrentVolume; set => mediaController.SetVolumeTo(value, 0); }

        public int MaxVolume { get => mediaController.GetPlaybackInfo().MaxVolume; set => throw new System.NotImplementedException(); }

        public bool Muted { get => mediaManagerImplementation.MediaBrowserManager.mediaController.GetPlaybackInfo().CurrentVolume == 0; set => mediaController.SetVolumeTo(0, 0); }

        public event VolumeChangedEventHandler VolumeChanged;
    }
}

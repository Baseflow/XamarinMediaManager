using Android.OS;

namespace Plugin.MediaManager.MediaPlayerService
{
    public class MediaPlayerServiceBinder : Binder
    {
        private MediaPlayerService service;

        public MediaPlayerServiceBinder(MediaPlayerService service)
        {
            this.service = service;
        }

        public MediaPlayerService GetMediaPlayerService()
        {
            return service;
        }
    }
}


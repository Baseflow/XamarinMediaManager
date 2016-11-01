using Android.OS;

namespace Plugin.MediaManager
{
    public class MediaServiceBinder : Binder
    {
        private MediaServiceBase service;

        public MediaServiceBinder(MediaServiceBase service)
        {
            this.service = service;
        }

        public MediaServiceBase GetMediaPlayerService()
        {
            return service;
        }
    }
}


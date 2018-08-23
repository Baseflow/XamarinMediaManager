using Android.OS;

namespace MediaManager.Platforms.Android.ServiceBinder
{
    public class MediaBrowserServiceBinder : Binder
    {
        private MediaBrowserService service;

        public MediaBrowserServiceBinder(MediaBrowserService service)
        {
            this.service = service;
        }

        public MediaBrowserService GetMediaPlayerService()
        {
            return service;
        }
    }
}

using Android.OS;

namespace Plugin.MediaManager
{
    public class MediaServiceBinder : Binder { 
        private MediaServiceBase service;

        public MediaServiceBinder(MediaServiceBase service)
        {
            this.service = service;
        }

        public TService GetMediaPlayerService<TService>() where TService : MediaServiceBase
        {
            return service as TService;
        }
    }
}


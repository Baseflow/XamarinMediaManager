using Android.Content;
using Android.OS;

namespace MediaManager.Platforms.Android.ServiceBinder
{
    internal class MediaBrowserServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private MediaManagerImplementation mediaManager;

        public MediaBrowserServiceConnection(MediaManagerImplementation mediaManager)
        {
            this.mediaManager = mediaManager;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var instance = mediaManager as MediaManagerImplementation;

            var mediaPlayerServiceBinder = service as MediaBrowserServiceBinder;
            if (mediaPlayerServiceBinder != null)
            {
                instance?.OnServiceConnected(mediaPlayerServiceBinder);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            var instance = mediaManager as MediaManagerImplementation;
            instance?.OnServiceDisconnected();
        }
    }
}

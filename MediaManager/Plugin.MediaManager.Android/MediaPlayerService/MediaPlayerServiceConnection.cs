using Android.Content;
using Android.OS;
using Plugin.MediaManager.Audio;

namespace Plugin.MediaManager.MediaPlayerService
{
    internal class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private AudioPlayerImplementation instance;

        public MediaPlayerServiceConnection(AudioPlayerImplementation mediaPlayer)
        {
            this.instance = mediaPlayer;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var mediaPlayerServiceBinder = service as MediaPlayerServiceBinder;
            if (mediaPlayerServiceBinder != null)
            {
               instance.OnServiceConnected(mediaPlayerServiceBinder);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            instance.OnServiceDisconnected();
        }
    }
}
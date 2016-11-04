using Android.Content;
using Android.OS;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.ExoPlayer;

namespace Plugin.MediaManager
{
    internal class MediaServiceConnection<TService> : Java.Lang.Object, IServiceConnection where TService : MediaServiceBase
    {
        private IAudioPlayer player;

        public MediaServiceConnection(IAudioPlayer mediaPlayer)
        {
            player = mediaPlayer;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var instance = player as AudioPlayerImplementation<TService>;
            var mediaPlayerServiceBinder = service as MediaServiceBinder;
            if (mediaPlayerServiceBinder != null)
            {
               instance?.OnServiceConnected(mediaPlayerServiceBinder);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            var instance = player as AudioPlayerImplementation<TService>;
            instance?.OnServiceDisconnected();
        }
    }
}
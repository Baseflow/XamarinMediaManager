using System;
using Android.Content;
using Android.OS;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class MediaPlayerServiceConnection: Java.Lang.Object, IServiceConnection
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
                var serviceBinder = (MediaPlayerServiceBinder)service;
                instance.Binder = serviceBinder;
                instance.isBound = true;

                if (instance.AlternateRemoteCallback != null)
                    serviceBinder.GetMediaPlayerService().AlternateRemoteCallback = instance.AlternateRemoteCallback;

                //serviceBinder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) => { instance.CoverReloaded?.Invoke(sender, e); };
                //serviceBinder.GetMediaPlayerService().StatusChanged += (object sender, StatusChangedEventArgs e) => { instance.StatusChanged?.Invoke(sender, e); };
                //serviceBinder.GetMediaPlayerService().Playing += (sender, args) => { instance.PlayingChanged?.Invoke(sender, args); };
                //serviceBinder.GetMediaPlayerService().Buffering += (sender, args) => { instance.BufferingChanged?.Invoke(sender, args); };
                //serviceBinder.GetMediaPlayerService().MediaFinished += (sender, args) => { instance.MediaFinished?.Invoke(sender, args); };
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            instance.isBound = false;
        }
    }
}

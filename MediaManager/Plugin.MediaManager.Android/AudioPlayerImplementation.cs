using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class AudioPlayerImplementation : IAudioPlayer
    {
        private Context applicationContext;

        public bool isBound = false;
        private MediaPlayerServiceBinder binder;
        public MediaPlayerServiceBinder Binder
        {
            get
            {
                return binder;
            }
            set
            {
                binder = value;
            }
        }

        public AudioPlayerImplementation()
        {
            applicationContext = Application.Context;

            mediaPlayerServiceIntent = new Intent(applicationContext, typeof(MediaPlayerService));
            mediaPlayerServiceConnection = new MediaPlayerServiceConnection(this);
            applicationContext.BindService(mediaPlayerServiceIntent, mediaPlayerServiceConnection, Bind.AutoCreate);
        }

        private MediaPlayerServiceConnection mediaPlayerServiceConnection;
        private Intent mediaPlayerServiceIntent;
        public MediaSessionCompat.Callback AlternateRemoteCallback { get; set; }

        private async Task BinderReady()
        {
            int repeat = 10;
            while ((binder == null) && (repeat > 0))
            {
                await Task.Delay(100);
                repeat--;
            }
            if (repeat == 0)
            {
                throw new System.TimeoutException("Could not connect to service");
            }
        }

        public void UnbindMediaPlayerService()
        {
            if (isBound)
            {
                binder.GetMediaPlayerService().StopNotification();
                applicationContext.UnbindService(mediaPlayerServiceConnection);

                isBound = false;
            }
        }

        public TimeSpan Position => binder.GetMediaPlayerService().Position;
        public TimeSpan Duration => binder.GetMediaPlayerService().Duration;
        public TimeSpan Buffered => binder.GetMediaPlayerService().Buffered;

        public MediaPlayerStatus Status => binder.GetMediaPlayerService().Status;

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        public async Task Pause()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Pause();
        }

        public async Task Play(IMediaFile mediaFile)
        {
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                    await BinderReady();
                    await binder.GetMediaPlayerService().Play(mediaFile.Url, MediaFileType.AudioUrl);
                    break;
                case MediaFileType.AudioFile:
                    await BinderReady();
                    await binder.GetMediaPlayerService().Play(mediaFile.Url, MediaFileType.AudioFile);
                    break;
                default:
                    await Task.FromResult(0);
                    break;
            }
        }

        public async Task Play(string url, MediaFileType fileType)
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Play(url, fileType);
        }

        public async Task PlayPause()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().PlayPause();
        }

        public async Task Seek(TimeSpan position)
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Seek(position);
        }

        public async Task Stop()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Stop();
        }

        private class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
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
                    serviceBinder.GetMediaPlayerService().StatusChanged += (object sender, StatusChangedEventArgs e) => { instance.StatusChanged?.Invoke(sender, e); };
                    serviceBinder.GetMediaPlayerService().PlayingChanged += (sender, args) => { instance.PlayingChanged?.Invoke(sender, args); };
                    serviceBinder.GetMediaPlayerService().BufferingChanged += (sender, args) => { instance.BufferingChanged?.Invoke(sender, args); };
                    serviceBinder.GetMediaPlayerService().MediaFinished += (sender, args) => { instance.MediaFinished?.Invoke(sender, args); };
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                instance.isBound = false;
            }
        }
    }
}

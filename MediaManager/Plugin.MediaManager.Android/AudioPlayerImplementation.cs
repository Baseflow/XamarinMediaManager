using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
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
    }
}

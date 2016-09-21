using Plugin.MediaManager.Abstractions;
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using Java.Util.Concurrent;

namespace Plugin.MediaManager
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : IMediaManager
    {
        private Context applicationContext;

        private bool isBound = false;
        private MediaPlayerServiceBinder binder;
        private MediaPlayerServiceConnection mediaPlayerServiceConnection;
        private Intent mediaPlayerServiceIntent;

        public MediaManagerImplementation()
        {
            applicationContext = Application.Context;

            mediaPlayerServiceIntent = new Intent(applicationContext, typeof(MediaPlayerService));
            mediaPlayerServiceConnection = new MediaPlayerServiceConnection(this);
            applicationContext.BindService(mediaPlayerServiceIntent, mediaPlayerServiceConnection, Bind.AutoCreate);
        }

        public IMediaQueue Queue
        {
            get
            {
                return binder.GetMediaPlayerService().Queue;
            }

            set
            {
                binder.GetMediaPlayerService().Queue = value;
            }
        }

        public event StatusChangedEventHandler StatusChanged;

        public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingEventHandler Playing;

        public event BufferingEventHandler Buffering;

        public event TrackFinishedEventHandler TrackFinished;


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


        public async Task Play(string url)
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Play(url, MediaFileType.AudioUrl);
        }


        public async Task Stop()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Stop();
        }

        public async Task Pause()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Pause();
        }

        public async Task Seek(int position)
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Seek(position);
        }

        public async Task PlayNext()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().PlayNext();
        }

        public async Task PlayPrevious()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().PlayPrevious();
        }

        public async Task PlayPause()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().PlayPause();
        }

        /*public async Task PlayByPosition(int index)
        {
            await binder.GetMediaPlayerService().PlayByPosition(index);
        }*/

        public PlayerStatus Status
        {
            get
            {
                return binder.GetMediaPlayerService().Status;
            }
        }

        public int Position
        {
            get
            {
                return binder.GetMediaPlayerService().Position;
            }
        }

        public int Duration
        {
            get
            {
                return binder.GetMediaPlayerService().Duration;
            }
        }

        public int Buffered
        {
            get
            {
                return binder.GetMediaPlayerService().Buffered;
            }
        }

        public object Cover
        {
            get
            {
                return binder.GetMediaPlayerService().Cover;
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

        public async Task Play(IMediaFile mediaFile)
        {
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                    await BinderReady();
                    await binder.GetMediaPlayerService().Play(mediaFile.Url, MediaFileType.AudioUrl);
                    break;
                case MediaFileType.VideoUrl:
                    throw new NotImplementedException();
                    break;
                case MediaFileType.AudioFile:
                    await BinderReady();
                    await binder.GetMediaPlayerService().Play(mediaFile.Url, MediaFileType.AudioFile);
                    break;
                case MediaFileType.VideoFile:
                    throw new NotImplementedException();
                    break;
                case MediaFileType.Other:
                    throw new NotImplementedException();
                    break;
                default:
                    await Task.FromResult(0);
                    break;
            }
        }

        public MediaSessionCompat.Callback AlternateRemoteCallback { get; set; }

        private class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
        {
            private MediaManagerImplementation instance;

            public MediaPlayerServiceConnection(MediaManagerImplementation mediaPlayer)
            {
                this.instance = mediaPlayer;
            }

            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                var mediaPlayerServiceBinder = service as MediaPlayerServiceBinder;
                if (mediaPlayerServiceBinder != null)
                {
                    var binder = (MediaPlayerServiceBinder)service;
                    instance.binder = binder;
                    instance.isBound = true;

                    if (instance.AlternateRemoteCallback != null)
                        binder.GetMediaPlayerService().AlternateRemoteCallback = instance.AlternateRemoteCallback;

                    binder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) => { if (instance.CoverReloaded != null) instance.CoverReloaded(sender, e); };
                    binder.GetMediaPlayerService().StatusChanged += (object sender, EventArgs e) => { if (instance.StatusChanged != null) instance.StatusChanged(sender, e); };
                    binder.GetMediaPlayerService().Playing += (object sender, EventArgs e) => { if (instance.Playing != null) instance.Playing(sender, e); };
                    binder.GetMediaPlayerService().Buffering += (object sender, EventArgs e) => { if (instance.Buffering != null) instance.Buffering(sender, e); };
                    binder.GetMediaPlayerService().TrackFinished += (object sender, EventArgs e) => { if (instance.TrackFinished != null) instance.TrackFinished(sender, e); };
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                instance.isBound = false;
            }
        }
    }
}
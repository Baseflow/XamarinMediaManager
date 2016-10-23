using Plugin.MediaManager.Abstractions;
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : IMediaManager
    {
        private Context applicationContext;

        private bool isBound = false;
        private MediaPlayerServiceBinder binder;
        public MediaPlayerServiceBinder Binder { 
            get {
                return binder;
            } 
        }

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

        //public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingChangedEventHandler Playing;

        public event BufferingChangedEventHandler Buffering;

        public event MediaFinishedEventHandler TrackFinished;

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

        public MediaPlayerStatus Status => binder.GetMediaPlayerService().Status;

        public int Position => binder.GetMediaPlayerService().Position;

        public int Duration => binder.GetMediaPlayerService().Duration;

        public int Buffered => binder.GetMediaPlayerService().Buffered;

        public object Cover => binder.GetMediaPlayerService().Cover;

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
                case MediaFileType.AudioFile:
                    await BinderReady();
                    await binder.GetMediaPlayerService().Play(mediaFile.Url, MediaFileType.AudioFile);
                    break;
                case MediaFileType.VideoFile:
                    throw new NotImplementedException();
                case MediaFileType.Other:
                    throw new NotImplementedException();
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
                    var serviceBinder = (MediaPlayerServiceBinder)service;
                    instance.binder = serviceBinder;
                    instance.isBound = true;

                    if (instance.AlternateRemoteCallback != null)
                        serviceBinder.GetMediaPlayerService().AlternateRemoteCallback = instance.AlternateRemoteCallback;

                    //serviceBinder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) => { instance.CoverReloaded?.Invoke(sender, e); };
                    serviceBinder.GetMediaPlayerService().StatusChanged += (object sender, StatusChangedEventArgs e) => { instance.StatusChanged?.Invoke(sender, e); };
                    serviceBinder.GetMediaPlayerService().Playing += (sender, args) => { instance.Playing?.Invoke(sender, args); };
                    serviceBinder.GetMediaPlayerService().Buffering += (sender, args) => { instance.Buffering?.Invoke(sender, args); };
                    serviceBinder.GetMediaPlayerService().MediaFinished += (object sender, EventArgs e) => { instance.TrackFinished?.Invoke(sender, e); };
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                instance.isBound = false;
            }
        }
    }
}
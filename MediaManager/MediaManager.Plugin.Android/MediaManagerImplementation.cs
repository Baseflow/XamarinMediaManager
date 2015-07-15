using MediaManager.Plugin.Abstractions;
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;


namespace MediaManager.Plugin
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
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

        public event StatusChangedEventHandler StatusChanged;

        public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingEventHandler Playing;

        public event BufferingEventHandler Buffering;

        public async Task Play()
        {
            await binder.GetMediaPlayerService().Play();
        }

        public async Task Stop()
        {
            await binder.GetMediaPlayerService().Stop();
        }

        public async Task Pause()
        {
            await binder.GetMediaPlayerService().Pause();
        }

        public async Task Seek(int position)
        {
            await binder.GetMediaPlayerService().Seek(position);
        }

        public async Task PlayNext()
        {
            await binder.GetMediaPlayerService().PlayNext();
        }

        public async Task PlayPause()
        {
            await binder.GetMediaPlayerService().PlayPause();
        }

        public async Task PlayPrevious()
        {
            await binder.GetMediaPlayerService().PlayPrevious();
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

                    binder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) => { if (instance.CoverReloaded != null) instance.CoverReloaded(sender, e); };
                    binder.GetMediaPlayerService().StatusChanged += (object sender, EventArgs e) => { if (instance.StatusChanged != null) instance.StatusChanged(sender, e); };
                    binder.GetMediaPlayerService().Playing += (object sender, EventArgs e) => { if (instance.Playing != null) instance.Playing(sender, e); };
                    binder.GetMediaPlayerService().Buffering += (object sender, EventArgs e) => { if (instance.Buffering != null) instance.Buffering(sender, e); };
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                instance.isBound = false;
            }
        }
    }
}
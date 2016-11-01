using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class AudioPlayerImplementation<TService> : IAudioPlayer where TService : MediaServiceBase
    {
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFileFailedEventHandler MediaFileFailed;

        public Context applicationContext;
        private MediaServiceConnection mediaPlayerServiceConnection;
        private Intent mediaPlayerServiceIntent;
        private MediaSessionManagerImplementation _sessionManager;

        private bool isBound;

        public TimeSpan Position => binder.GetMediaPlayerService().Position;

        public TimeSpan Duration => binder.GetMediaPlayerService().Duration;

        public TimeSpan Buffered => binder.GetMediaPlayerService().Buffered;

        public MediaSessionCompat.Callback AlternateRemoteCallback { get; set; }

        private MediaPlayerStatus status;
        public virtual MediaPlayerStatus Status
        {
            get
            {
                if(!isBound) return MediaPlayerStatus.Stopped;
                var state = binder.GetMediaPlayerService().MediaPlayerState;
                return GetStatusByCompatValue(state);
            }
            private set
            {
                status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(status));
            }
        }

        private MediaServiceBinder binder;
        public MediaServiceBinder Binder
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

        public AudioPlayerImplementation(MediaSessionManagerImplementation sessionManager)
        {
            _sessionManager = sessionManager;
            applicationContext = Application.Context;
            mediaPlayerServiceIntent = GetMediaServiceIntent();
            mediaPlayerServiceConnection = new MediaServiceConnection(this);
            var success = applicationContext.BindService(mediaPlayerServiceIntent, mediaPlayerServiceConnection, Bind.AutoCreate);
            _sessionManager.OnStatusChanged += (sender, i) => Status = GetStatusByCompatValue(i);
            StatusChanged += (sender, args) =>
            {
                if (args.Status == MediaPlayerStatus.Playing)
                    Task.Run(() => OnPlaying());
            };
            
        }

        public Intent GetMediaServiceIntent()
        {
            return new Intent(applicationContext, typeof(MediaPlayerService));
        }

        public void UnbindMediaPlayerService()
        {
            if (isBound)
            {
                applicationContext.UnbindService(mediaPlayerServiceConnection);
                isBound = false;
            }
        }

        public virtual async Task Pause()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Pause();
        }

        public virtual async Task Play(IMediaFile mediaFile)
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Play(mediaFile);
        }

        public virtual async Task Seek(TimeSpan position)
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Seek(position);
        }

        public virtual async Task Stop()
        {
            await BinderReady();
            await binder.GetMediaPlayerService().Stop();
        }

        internal void OnServiceConnected(MediaServiceBinder serviceBinder)
        {
            Binder = serviceBinder;
            isBound = true;

            if (AlternateRemoteCallback != null)
                serviceBinder.GetMediaPlayerService().AlternateRemoteCallback = AlternateRemoteCallback;

            //serviceBinder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) => { instance.CoverReloaded?.Invoke(sender, e); };
            serviceBinder.GetMediaPlayerService().StatusChanged += (object sender, StatusChangedEventArgs e) => { StatusChanged?.Invoke(this, e); };
            serviceBinder.GetMediaPlayerService().PlayingChanged += (sender, args) => { PlayingChanged?.Invoke(this, args); };
            serviceBinder.GetMediaPlayerService().BufferingChanged += (sender, args) => { BufferingChanged?.Invoke(this, args); };
            serviceBinder.GetMediaPlayerService().MediaFinished += (sender, args) => { MediaFinished?.Invoke(this, args); };
            serviceBinder.GetMediaPlayerService().SetMediaSession(_sessionManager);
        }

        internal void OnServiceDisconnected()
        {
            isBound = false;
        }

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

        public void OnPlaying()
        {
            var progress = (Position.TotalSeconds/Duration.TotalSeconds) * 100;
            var position = Position;
            var duration = Duration;
           
            if (Status == MediaPlayerStatus.Playing)
                Task.Delay(1000).ContinueWith(task => OnPlaying());

            if (Status == MediaPlayerStatus.Stopped || Status == MediaPlayerStatus.Failed)
            {
                duration = TimeSpan.Zero;
                position = TimeSpan.Zero;
                progress = 0;
            }

            PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(
                progress >= 0 ? progress : 0, 
                position.TotalSeconds >= 0 ? position : TimeSpan.Zero, 
                duration.TotalSeconds >= 0 ? duration : TimeSpan.Zero));
        }

        public MediaPlayerStatus GetStatusByCompatValue(int state)
        {
            switch (state)
            {
                case PlaybackStateCompat.StateFastForwarding:
                case PlaybackStateCompat.StateRewinding:
                case PlaybackStateCompat.StateSkippingToNext:
                case PlaybackStateCompat.StateSkippingToPrevious:
                case PlaybackStateCompat.StateSkippingToQueueItem:
                case PlaybackStateCompat.StatePlaying:
                    return MediaPlayerStatus.Playing;

                case PlaybackStateCompat.StatePaused:
                    return MediaPlayerStatus.Paused;

                case PlaybackStateCompat.StateConnecting:
                case PlaybackStateCompat.StateBuffering:
                    return MediaPlayerStatus.Buffering;

                case PlaybackStateCompat.StateError:
                case PlaybackStateCompat.StateStopped:
                    return MediaPlayerStatus.Stopped;

                default:
                    return MediaPlayerStatus.Stopped;
            }
        }

    }
}

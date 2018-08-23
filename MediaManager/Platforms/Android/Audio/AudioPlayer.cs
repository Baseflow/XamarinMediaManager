using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Android.Audio;

namespace MediaManager
{
    public class AudioPlayer : Java.Lang.Object, IAudioPlayer, Platforms.Android.Utils.IExoPlayerPlayer
    {
        private MediaSessionCompat _mediaSession;
        public AudioPlayer(MediaSessionCompat mediaSession)
        {
            _mediaSession = mediaSession;
        }

        public SimpleExoPlayer Player { get; internal set; }
        string userAgent;

        private DefaultHttpDataSourceFactory defaultHttpDataSourceFactory;
        private DefaultDataSourceFactory defaultDataSourceFactory;
        private DefaultBandwidthMeter defaultBandwidthMeter;
        private AdaptiveTrackSelection.Factory adaptiveTrackSelectionFactory;
        private DefaultTrackSelector defaultTrackSelector;
        private MediaSessionConnector connector;
        private AudioFocusManager audioFocusManager;
        private ConcatenatingMediaSource mediaSource;
        private MediaQueue q;

        public Dictionary<string, string> RequestHeaders { get; set; }

        public MediaPlayerStatus Status
        {
            get
            {
                if (!_mediaSession.Active) return MediaPlayerStatus.Stopped;

                return GetStatusByCompatValue((int)_mediaSession.Controller.PlaybackState.State);
            }
        }

        public TimeSpan Position => TimeSpan.FromTicks(Player.CurrentPosition);

        public TimeSpan Duration => TimeSpan.FromTicks(Player.Duration);

        public TimeSpan Buffered => TimeSpan.FromTicks(Player.BufferedPosition);

        protected Context Context { get; set; } = Application.Context;

        public Task Pause()
        {
            Player.Stop();
            return Task.CompletedTask;
        }

        public void Initialize()
        {
            if (Player != null)
                return;

            userAgent = Util.GetUserAgent(Context, "MediaManager");
            defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            defaultDataSourceFactory = new DefaultDataSourceFactory(Context, null, defaultHttpDataSourceFactory);
            defaultBandwidthMeter = new DefaultBandwidthMeter();
            adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            mediaSource = new ConcatenatingMediaSource();

            Com.Google.Android.Exoplayer2.Audio.AudioAttributes mAudioAttributes = new Com.Google.Android.Exoplayer2.Audio.AudioAttributes.Builder()
               .SetUsage((int)AudioUsageKind.Media)
               .SetContentType((int)AudioContentType.Music)
               .Build();

            audioFocusManager = new AudioFocusManager(this);

            Player = ExoPlayerFactory.NewSimpleInstance(Context, defaultTrackSelector);
            Player.AddListener(new PlayerEventListener());
            Player.AudioAttributes = mAudioAttributes;

            connector = new MediaSessionConnector(_mediaSession, new PlaybackController(audioFocusManager));
            connector.SetPlayer(Player, new PlaybackPreparer(Player, defaultDataSourceFactory, audioFocusManager), null);

            //Player.PlayWhenReady = true;
        }

        public Task Play(string Url)
        {
            //var mediaUri = Android.Net.Uri.Parse(Url);

            //var extractorMediaSource = new ExtractorMediaSource(mediaUri, defaultDataSourceFactory, new DefaultExtractorsFactory(), null, null);
            //Player.Prepare(extractorMediaSource);
            //Player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Play()
        {
            Player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Seek(TimeSpan position)
        {
            //Player.SeekTo((long)position.TotalMilliseconds);

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            //Player.Stop(true);

            return Task.CompletedTask;
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

        internal void SetQueue(MediaQueue mediaQueue)
        {
            q = mediaQueue;

            connector.SetQueueEditor(new TimelineQueueEditor(_mediaSession.Controller, mediaSource, new DataAdapter(q), new SourceFactory(defaultDataSourceFactory)));
            connector.SetQueueNavigator(new QueueNavigator(_mediaSession));
        }

        private class PlaybackController : DefaultPlaybackController
        {
            private AudioFocusManager audioFocusManager;

            public PlaybackController(AudioFocusManager audioFocusManager)
            {
                this.audioFocusManager = audioFocusManager;
            }

            public PlaybackController(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public override void OnPause(IPlayer player)
            {
                audioFocusManager.AbandonAudioFocus();
            }

            public override void OnPlay(IPlayer player)
            {
                audioFocusManager.RequestAudioFocus();
            }

            public override void OnStop(IPlayer player)
            {
                audioFocusManager.AbandonAudioFocus();
                player.Stop();
            }
        }

        private class DataAdapter : Java.Lang.Object, TimelineQueueEditor.IQueueDataAdapter
        {
            private MediaQueue q;

            public DataAdapter(MediaQueue q)
            {
                this.q = q;
            }

            public void Add(int index, MediaDescriptionCompat description)
            {
                //TODO: generate IMediaItem from description.
                q.Insert(index, null);
            }

            public MediaDescriptionCompat GetMediaDescription(int index)
            {
                return q[index].GetDescriptionCompat();
            }

            public void Move(int oldIndex, int newIndex)
            {
                q.Move(oldIndex, newIndex);
            }

            public void Remove(int index)
            {
                q.RemoveAt(index);
            }
        }

        private class SourceFactory : Java.Lang.Object, TimelineQueueEditor.IMediaSourceFactory
        {
            private DefaultDataSourceFactory factory;

            public SourceFactory(DefaultDataSourceFactory factory)
            {
                this.factory = factory;
            }

            public IMediaSource CreateMediaSource(MediaDescriptionCompat description)
            {
                IMediaSource src = null;

                if (description.MediaId != null)
                    src = null; //TODO: Implement preparefrommediasource

                else if (description.MediaUri != null)
                    src = new ExtractorMediaSource(description.MediaUri, factory, new DefaultExtractorsFactory(), null, null);

                return src;
            }
        }

        private class QueueNavigator : TimelineQueueNavigator
        {
            Timeline.Window window = new Timeline.Window();
            MediaSessionCompat mediaSession;

            public QueueNavigator(MediaSessionCompat mediaSession) : base(mediaSession)
            {
                this.mediaSession = mediaSession;
            }

            public QueueNavigator(MediaSessionCompat mediaSession, int maxQueueSize) : base(mediaSession, maxQueueSize)
            {
            }

            protected QueueNavigator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public override MediaDescriptionCompat GetMediaDescription(IPlayer player, int windowIndex)
            {
                return player.CurrentTimeline.GetWindow(windowIndex, window, true).Tag as MediaDescriptionCompat;
            }
        }

        private class PlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
        {
            private SimpleExoPlayer _player;
            private DefaultDataSourceFactory _dataSourceFactory;
            private AudioFocusManager audioFocusManager;

            public PlaybackPreparer(SimpleExoPlayer player, DefaultDataSourceFactory dataSourceFactory, AudioFocusManager audioFocusManager)
            {
                _player = player;
                _dataSourceFactory = dataSourceFactory;
                this.audioFocusManager = audioFocusManager;
            }

            public PlaybackPreparer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public long SupportedPrepareActions =>
                    PlaybackStateCompat.ActionPrepare |
                    PlaybackStateCompat.ActionPrepareFromMediaId |
                    PlaybackStateCompat.ActionPrepareFromSearch |
                    PlaybackStateCompat.ActionPrepareFromUri;

            public string[] GetCommands()
            {
                return null;
            }

            public void OnCommand(IPlayer p0, string p1, Bundle p2, ResultReceiver p3)
            {
                ;
            }

            public void OnPrepare()
            {
                ;
            }

            public void OnPrepareFromMediaId(string p0, Bundle p1)
            {
                ;
            }

            public void OnPrepareFromSearch(string p0, Bundle p1)
            {
                ;
            }

            public void OnPrepareFromUri(Android.Net.Uri mediaUri, Bundle p1)
            {
                var extractorMediaSource = new ExtractorMediaSource(mediaUri, _dataSourceFactory, new DefaultExtractorsFactory(), null, null);
                _player.Prepare(extractorMediaSource);
            }
        }

        private class PlayerEventListener : PlayerDefaultEventListener
        {
            public PlayerEventListener()
            {
            }

            public PlayerEventListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }
        }
    }
}

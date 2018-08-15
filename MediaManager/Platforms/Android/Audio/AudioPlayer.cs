using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
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

namespace MediaManager
{
    public class AudioPlayer : Java.Lang.Object, IAudioPlayer
    {
        private MediaSessionCompat _mediaSession;
        public AudioPlayer(MediaSessionCompat mediaSession)
        {
            _mediaSession = mediaSession;
        }

        protected SimpleExoPlayer _player;
        string userAgent;

        private DefaultHttpDataSourceFactory defaultHttpDataSourceFactory;
        private DefaultDataSourceFactory defaultDataSourceFactory;
        private DefaultBandwidthMeter defaultBandwidthMeter;
        private AdaptiveTrackSelection.Factory adaptiveTrackSelectionFactory;
        private DefaultTrackSelector defaultTrackSelector;
        private MediaSessionConnector connector;

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaPlayerStatus Status
        {
            get
            {
                if (!_mediaSession.Active) return MediaPlayerStatus.Stopped;

                return GetStatusByCompatValue((int)_mediaSession.Controller.PlaybackState);
            }
        }

        public TimeSpan Position => TimeSpan.FromTicks(_player.CurrentPosition);

        public TimeSpan Duration => TimeSpan.FromTicks(_player.Duration);

        public TimeSpan Buffered => TimeSpan.FromTicks(_player.BufferedPosition);

        protected Context Context { get; set; } = Application.Context;

        public Task Pause()
        {
            _player.Stop();
            return Task.CompletedTask;
        }

        public void Initialize()
        {
            if (_player != null)
                return;

            userAgent = Util.GetUserAgent(Context, "MediaManager");
            defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            defaultDataSourceFactory = new DefaultDataSourceFactory(Context, null, defaultHttpDataSourceFactory);
            defaultBandwidthMeter = new DefaultBandwidthMeter();
            adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            _player = ExoPlayerFactory.NewSimpleInstance(Context, defaultTrackSelector);
            _player.AddListener(new PlayerEventListener());

            connector = new MediaSessionConnector(_mediaSession, new PlaybackController());
            connector.SetPlayer(_player, new PlaybackPreparer(_player, defaultDataSourceFactory), null);
            _player.PlayWhenReady = true;
        }

        public Task Play(string Url)
        {
            var mediaUri = Android.Net.Uri.Parse(Url);

            var extractorMediaSource = new ExtractorMediaSource(mediaUri, defaultDataSourceFactory, new DefaultExtractorsFactory(), null, null);
            _player.Prepare(extractorMediaSource);
            _player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Play()
        {
            if (_player != null)
                _player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Seek(TimeSpan position)
        {
            _player.SeekTo((long)position.TotalMilliseconds);

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            _player.Stop(true);

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

        private class PlaybackController : DefaultPlaybackController
        {
            public PlaybackController()
            {
            }

            public PlaybackController(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }
        }

        private class PlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
        {
            private SimpleExoPlayer _player;
            private DefaultDataSourceFactory _dataSourceFactory;

            public PlaybackPreparer(SimpleExoPlayer player, DefaultDataSourceFactory dataSourceFactory)
            {
                _player = player;
                _dataSourceFactory = dataSourceFactory;
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
                _player.PlayWhenReady = true;
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

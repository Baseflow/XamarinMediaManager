using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
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

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaPlayerStatus Status
        {
            get
            {
                if (!_mediaSession.Active) return MediaPlayerStatus.stopped;

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

        protected Task Initialize()
        {
            if (_player != null)
                return Task.CompletedTask;

            userAgent = Util.GetUserAgent(Context, "MediaManager");
            defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            defaultDataSourceFactory = new DefaultDataSourceFactory(Context, null, defaultHttpDataSourceFactory);
            defaultBandwidthMeter = new DefaultBandwidthMeter();
            adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            _player = ExoPlayerFactory.NewSimpleInstance(Context, defaultTrackSelector);
            _player.AddListener(new PlayerEventListener());
            MediaSessionConnector mediaSessionConnector = new MediaSessionConnector(_mediaSession);
            mediaSessionConnector.SetPlayer(_player, null, null);

            return Task.CompletedTask;
        }

        public async Task Play(string Url)
        {
            var mediaUri = Android.Net.Uri.Parse(Url);

            await Initialize();

            var extractorMediaSource = new ExtractorMediaSource(mediaUri, defaultDataSourceFactory, new DefaultExtractorsFactory(), null, null);
            _player.Prepare(extractorMediaSource);
            _player.PlayWhenReady = true;
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
                    return MediaPlayerStatus.playing;

                case PlaybackStateCompat.StatePaused:
                    return MediaPlayerStatus.paused;

                case PlaybackStateCompat.StateConnecting:
                case PlaybackStateCompat.StateBuffering:
                    return MediaPlayerStatus.buffering;

                case PlaybackStateCompat.StateError:
                case PlaybackStateCompat.StateStopped:
                    return MediaPlayerStatus.stopped;

                default:
                    return MediaPlayerStatus.stopped;
            }
        }

        private class PlayerEventListener : PlayerDefaultEventListener
        {
            public override void OnPlayerStateChanged(bool playWhenReady, int playbackState)
            {
                base.OnPlayerStateChanged(playWhenReady, playbackState);
            }
        }
    }
}

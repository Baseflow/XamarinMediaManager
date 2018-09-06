using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Java.Lang;
using Java.Util.Concurrent;
using MediaManager.Abstractions.Enums;
using MediaManager.Audio;
using MediaManager.Platforms.Android.Audio;
using MediaManager.Platforms.Android.Utils;

namespace MediaManager
{
    public class AudioPlayer : Java.Lang.Object, IAudioPlayer, IExoPlayerPlayer
    {
        public AudioPlayer()
        {
            StatusTimer.Elapsed += (object sender, ElapsedEventArgs e) => { OnPlaying(); };
            BufferedTimer.Elapsed += (object sender, ElapsedEventArgs e) => { OnBuffering(); };

            StatusTimer.AutoReset = BufferedTimer.AutoReset = true;
        }

        public AudioPlayer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        private MediaSessionCompat _mediaSession;
        public IExoPlayer Player { get; internal set; }
        string userAgent;

        private DefaultHttpDataSourceFactory defaultHttpDataSourceFactory;
        private DefaultDataSourceFactory defaultDataSourceFactory;
        private DefaultBandwidthMeter defaultBandwidthMeter;
        private AdaptiveTrackSelection.Factory adaptiveTrackSelectionFactory;
        private DefaultTrackSelector defaultTrackSelector;
        private MediaSessionConnector connector;
        public AudioFocusManager audioFocusManager;
        private ConcatenatingMediaSource mediaSource;

        #region scheduled updates
        Timer StatusTimer = new Timer(1000), BufferedTimer = new Timer(1000);
        #endregion

        public MediaPlayerState State
        {
            get
            {
                if (!_mediaSession.Active)
                    return MediaPlayerState.Stopped;

                return _mediaSession.Controller.PlaybackState.ToMediaPlayerState();
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

        public void Initialize(MediaSessionCompat mediaSession = null)
        {
            if (mediaSession != null)
                _mediaSession = mediaSession;

            if (Player != null)
                return;

            userAgent = Util.GetUserAgent(Context, "MediaManager");
            defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            defaultDataSourceFactory = new DefaultDataSourceFactory(Context, null, defaultHttpDataSourceFactory);
            defaultBandwidthMeter = new DefaultBandwidthMeter();
            adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);
            mediaSource = new ConcatenatingMediaSource();

            var audioAttributes = new Com.Google.Android.Exoplayer2.Audio.AudioAttributes.Builder()
               .SetUsage((int)AudioUsageKind.Media)
               .SetContentType((int)AudioContentType.Music)
               .Build();

            audioFocusManager = new AudioFocusManager(this);

            Player = ExoPlayerFactory.NewSimpleInstance(Context, defaultTrackSelector);
            Player.AddListener(new PlayerEventListener(this));
            if (Player is SimpleExoPlayer exoPlayer)
                exoPlayer.AudioAttributes = audioAttributes;

            connector = new MediaSessionConnector(_mediaSession, new PlaybackController(audioFocusManager));
            connector.SetQueueNavigator(new QueueNavigator(_mediaSession));
            connector.SetQueueEditor(new TimelineQueueEditor(_mediaSession.Controller, mediaSource, new QueueDataAdapter(), new MediaSourceFactory(defaultDataSourceFactory)));

            connector.SetPlayer(Player, new PlaybackPreparer(Player, defaultDataSourceFactory, mediaSource), null);
            Player.Prepare(mediaSource);
            //Player.PlayWhenReady = true;
        }

        public Task Play(string Url)
        {
            return Task.CompletedTask;
        }

        public Task Play()
        {
            Player.PlayWhenReady = true;

            return Task.CompletedTask;
        }

        public Task Seek(TimeSpan position)
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            //Player.Stop(true);

            return Task.CompletedTask;
        }

        private class PlayerEventListener : PlayerDefaultEventListener
        {
            private AudioPlayer player;

            public PlayerEventListener(AudioPlayer player)
            {
                this.player = player;
            }

            public PlayerEventListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public override void OnTracksChanged(TrackGroupArray trackGroups, TrackSelectionArray trackSelections)
            {
                for (int i = 0; i < trackGroups.Length; i++)
                {
                    TrackGroup trackGroup = trackGroups.Get(i);
                    for (int j = 0; j < trackGroup.Length; j++)
                    {
                        Metadata trackMetadata = trackGroup.GetFormat(j).Metadata;
                        if (trackMetadata != null)
                        {
                            // We found metadata. Do something with it here!
                        }
                    }
                }
                base.OnTracksChanged(trackGroups, trackSelections);
            }

            public override void OnPlayerStateChanged(bool playWhenReady, int playbackState)
            {
                if (playWhenReady)
                {
                    switch (playbackState)
                    {
                        case Com.Google.Android.Exoplayer2.Player.StateBuffering:
                            player.BufferedTimer.Start();
                            player.StatusTimer.Start();
                            break;
                        case Com.Google.Android.Exoplayer2.Player.StateReady:
                            player.StatusTimer.Start();
                            break;
                        case Com.Google.Android.Exoplayer2.Player.StateEnded:
                        case Com.Google.Android.Exoplayer2.Player.StateIdle:
                            player.BufferedTimer.Stop();
                            player.StatusTimer.Stop();
                            break;
                    }
                }
                else
                {
                    player.BufferedTimer.Stop();
                    player.StatusTimer.Stop();
                }

                base.OnPlayerStateChanged(playWhenReady, playbackState);
            }
        }

        private void OnPlaying()
        {
            SimpleExoPlayer simpleExoPlayer = Player as SimpleExoPlayer;

            double progress = (simpleExoPlayer.CurrentPosition / simpleExoPlayer.Duration) * 100;
            TimeSpan duration = TimeSpan.FromMilliseconds(simpleExoPlayer.Duration);
            TimeSpan position = TimeSpan.FromMilliseconds(simpleExoPlayer.CurrentPosition);

            CrossMediaManager.Current.OnPlayingChanged(this, new Abstractions.EventArguments.PlayingChangedEventArgs(progress, position, duration));
        }

        private void OnBuffering()
        {
            SimpleExoPlayer simpleExoPlayer = Player as SimpleExoPlayer;

            double progress = System.Math.Ceiling((double)(simpleExoPlayer.BufferedPosition / simpleExoPlayer.Duration) * 100);
            TimeSpan bufferedTime = TimeSpan.FromMilliseconds(simpleExoPlayer.BufferedPosition);

            CrossMediaManager.Current.OnBufferingChanged(this, new Abstractions.EventArguments.BufferingChangedEventArgs(progress, bufferedTime));

            if (progress == 100)
                BufferedTimer.Stop();
        }
    }
}

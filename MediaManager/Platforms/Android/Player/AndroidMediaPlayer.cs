using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using MediaManager.Media;
using MediaManager.Platforms.Android.Playback;
using MediaManager.Platforms.Android.Video;
using MediaManager.Playback;
using MediaManager.Video;

namespace MediaManager.Platforms.Android.Media
{
    public class AndroidMediaPlayer : Java.Lang.Object, IMediaPlayer<SimpleExoPlayer, VideoView>
    {
        public AndroidMediaPlayer()
        {
        }

        protected AndroidMediaPlayer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Android;

        protected Dictionary<string, string> RequestHeaders => MediaManager.RequestHeaders;

        protected Context Context => CrossMediaManager.Android.Context;

        public MediaPlayerState State
        {
            get
            {
                if (MediaSession == null || !MediaSession.Active)
                    return MediaPlayerState.Stopped;

                return MediaSession.Controller.PlaybackState.ToMediaPlayerState();
            }
        }

        protected string UserAgent { get; set; }
        protected DefaultHttpDataSourceFactory HttpDataSourceFactory { get; set; }
        public static DefaultDataSourceFactory DataSourceFactory { get; set; }
        public static DefaultDashChunkSource.Factory DashChunkSourceFactory { get; set; }
        public static DefaultSsChunkSource.Factory SsChunkSourceFactory { get; set; }

        protected DefaultBandwidthMeter BandwidthMeter { get; set; }
        protected AdaptiveTrackSelection.Factory TrackSelectionFactory { get; set; }
        protected DefaultTrackSelector TrackSelector { get; set; }
        protected PlaybackController PlaybackController { get; set; }

        protected MediaSessionConnector MediaSessionConnector { get; set; }
        protected QueueNavigator QueueNavigator { get; set; }
        protected ConcatenatingMediaSource MediaSource { get; set; }
        protected QueueDataAdapter QueueDataAdapter { get; set; }
        protected QueueEditorMediaSourceFactory MediaSourceFactory { get; set; }
        protected TimelineQueueEditor TimelineQueueEditor { get; set; }
        protected MediaSessionConnectorPlaybackPreparer PlaybackPreparer { get; set; }
        protected PlayerEventListener PlayerEventListener { get; set; }
        protected RatingCallback RatingCallback { get; set; }

        public SimpleExoPlayer Player { get; set; }
        public VideoView PlayerView { get; set; }
        public IVideoView VideoView => PlayerView;

        public MediaSessionCompat MediaSession { get; set; }

        public TimeSpan Position => TimeSpan.FromTicks(Player.CurrentPosition);

        public TimeSpan Duration => TimeSpan.FromTicks(Player.Duration);

        public TimeSpan Buffered => TimeSpan.FromTicks(Player.BufferedPosition);

        public RepeatMode RepeatMode
        {
            get
            {
                return (RepeatMode)Player.RepeatMode;
            }
            set
            {
                MediaManager.RepeatMode = value;
            }
        }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public virtual void Initialize()
        {
            if (Player != null)
                return;

            if (MediaSession == null)
                throw new ArgumentNullException(nameof(MediaSession));

            if (RequestHeaders?.Count > 0 && RequestHeaders.TryGetValue("User-Agent", out string userAgent))
                UserAgent = userAgent;
            else
                UserAgent = Util.GetUserAgent(Context, Context.PackageName);

            HttpDataSourceFactory = new DefaultHttpDataSourceFactory(UserAgent);

            if (RequestHeaders?.Count > 0)
            {
                foreach (var item in RequestHeaders)
                {
                    HttpDataSourceFactory.DefaultRequestProperties.Set(item.Key, item.Value);
                }
            }

            DataSourceFactory = new DefaultDataSourceFactory(Context, null, HttpDataSourceFactory);
            DashChunkSourceFactory = new DefaultDashChunkSource.Factory(DataSourceFactory);
            SsChunkSourceFactory = new DefaultSsChunkSource.Factory(DataSourceFactory);

            BandwidthMeter = new DefaultBandwidthMeter();
            TrackSelectionFactory = new AdaptiveTrackSelection.Factory(BandwidthMeter);
            TrackSelector = new DefaultTrackSelector(TrackSelectionFactory);
            MediaSource = new ConcatenatingMediaSource();

            Player = ExoPlayerFactory.NewSimpleInstance(Context, TrackSelector);

            var audioAttributes = new Com.Google.Android.Exoplayer2.Audio.AudioAttributes.Builder()
             .SetUsage(C.UsageMedia)
             .SetContentType(C.ContentTypeMusic)
             .Build();

            Player.AudioAttributes = audioAttributes;

            //TODO: Use this in 2.9.0
            //Player.SetAudioAttributes(audioAttributes, true);

            PlayerEventListener = new PlayerEventListener()
            {
                OnPlayerErrorImpl = (exception) =>
                {
                    MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, exception, exception.Message));
                },
                OnTracksChangedImpl = (trackGroups, trackSelections) =>
                {
                    var mediaItem = MediaManager.MediaQueue.Current;
                    BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));

                    MediaManager.MediaQueue.CurrentIndex = Player.CurrentWindowIndex;
                    MediaManager.OnMediaItemChanged(this, new MediaItemEventArgs(mediaItem));
                    
                    AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
                },
                OnPlayerStateChangedImpl = (bool playWhenReady, int playbackState) => {
                    switch (playbackState)
                    {
                        case Com.Google.Android.Exoplayer2.Player.StateEnded:
                            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));
                            break;
                        case Com.Google.Android.Exoplayer2.Player.StateIdle:
                        case Com.Google.Android.Exoplayer2.Player.StateBuffering:
                        case Com.Google.Android.Exoplayer2.Player.StateReady:
                        default:
                            break;
                    }
                }
            };
            Player.AddListener(PlayerEventListener);

            PlaybackController = new PlaybackController();
            MediaSessionConnector = new MediaSessionConnector(MediaSession, PlaybackController);

            QueueNavigator = new QueueNavigator(MediaSession);
            MediaSessionConnector.SetQueueNavigator(QueueNavigator);

            QueueDataAdapter = new QueueDataAdapter(MediaSource);
            MediaSourceFactory = new QueueEditorMediaSourceFactory();
            TimelineQueueEditor = new TimelineQueueEditor(MediaSession.Controller, MediaSource, QueueDataAdapter, MediaSourceFactory);
            MediaSessionConnector.SetQueueEditor(TimelineQueueEditor);

            RatingCallback = new RatingCallback();
            MediaSessionConnector.SetRatingCallback(RatingCallback);

            PlaybackPreparer = new MediaSessionConnectorPlaybackPreparer(Player, MediaSource);
            MediaSessionConnector.SetPlayer(Player, PlaybackPreparer, null);
        }

        public async Task Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));

            MediaSource.Clear();
            MediaSource.AddMediaSource(mediaItem.ToMediaSource());
            Player.Prepare(MediaSource);
            await Play();

            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public Task Play()
        {
            if (Player == null)
                return Task.CompletedTask;

            Player.PlayWhenReady = true;
            return Task.CompletedTask;
        }

        public Task Pause()
        {
            if (Player == null)
                return Task.CompletedTask;

            Player.Stop();
            return Task.CompletedTask;
        }

        public Task SeekTo(TimeSpan position)
        {
            if (Player == null)
                return Task.CompletedTask;

            Player.SeekTo(position.Milliseconds);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            if (Player == null)
                return Task.CompletedTask;

            Player.Stop();
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            Player.RemoveListener(PlayerEventListener);
            Player.Release();
            Player = null;

            base.Dispose(disposing);
        }
    }
}

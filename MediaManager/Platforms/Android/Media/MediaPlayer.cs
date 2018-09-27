using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Android.Audio;
using MediaManager.Platforms.Android.Playback;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Video;

namespace MediaManager.Platforms.Android.Media
{
    public abstract class MediaPlayer : Java.Lang.Object, IAudioPlayer, IVideoPlayer, IExoPlayerImplementation
    {
        public MediaPlayer()
        {
        }

        public MediaPlayer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        protected virtual int AudioAttributesContentType => (int)AudioContentType.Music;
        protected virtual int AudioAttributesUsage => (int)AudioUsageKind.Media;

        protected INotifyMediaManager MediaManager = CrossMediaManager.Current as INotifyMediaManager;

        protected Context Context { get; set; } = CrossMediaManager.Current.GetContext();

        protected string UserAgent { get; set; }
        protected DefaultHttpDataSourceFactory HttpDataSourceFactory { get; set; }
        public static DefaultDataSourceFactory DefaultDataSourceFactory { get; set; }
        public static DefaultDashChunkSource.Factory DefaultDashChunkSource { get; set; }
        public static DefaultSsChunkSource.Factory DefaultSsChunkSource { get; set; }

        protected DefaultBandwidthMeter BandwidthMeter { get; set; }
        protected AdaptiveTrackSelection.Factory TrackSelectionFactory { get; set; }
        protected DefaultTrackSelector TrackSelector { get; set; }
        protected PlaybackController PlaybackController { get; set; }

        protected MediaSessionConnector MediaSessionConnector { get; set; }
        protected QueueNavigator QueueNavigator { get; set; }
        protected ConcatenatingMediaSource MediaSource { get; set; }
        protected QueueDataAdapter QueueDataAdapter { get; set; }
        protected MediaSourceFactory MediaSourceFactory { get; set; }
        protected TimelineQueueEditor TimelineQueueEditor { get; set; }
        protected MediaSessionConnectorPlaybackPreparer PlaybackPreparer { get; set; }
        protected PlayerEventListener PlayerEventListener { get; set; }
        protected RatingCallback RatingCallback { get; set; }

        //TODO: Remove with Exoplayer 2.9.0
        internal AudioFocusManager AudioFocusManager { get; set; }

        public SimpleExoPlayer Player { get; set; }
        public MediaSessionCompat MediaSession { get; set; }

        public MediaPlayerState State
        {
            get
            {
                if (!MediaSession.Active)
                    return MediaPlayerState.Stopped;

                return MediaSession.Controller.PlaybackState.ToMediaPlayerState();
            }
        }

        public TimeSpan Position => TimeSpan.FromTicks(Player.CurrentPosition);

        public TimeSpan Duration => TimeSpan.FromTicks(Player.Duration);

        public TimeSpan Buffered => TimeSpan.FromTicks(Player.BufferedPosition);

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public virtual void Initialize()
        {
            if (Player != null)
                return;

            if (MediaSession == null)
                throw new ArgumentNullException(nameof(MediaSession));

            UserAgent = Util.GetUserAgent(Context, "MediaManager");
            HttpDataSourceFactory = new DefaultHttpDataSourceFactory(UserAgent);

            var requestHeaders = CrossMediaManager.Current.RequestHeaders;
            if (requestHeaders?.Count > 0)
            {
                foreach (var item in requestHeaders)
                {
                    HttpDataSourceFactory.DefaultRequestProperties.Set(item.Key, item.Value);
                }
            }

            DefaultDataSourceFactory = new DefaultDataSourceFactory(Context, null, HttpDataSourceFactory);
            DefaultDashChunkSource = new DefaultDashChunkSource.Factory(DefaultDataSourceFactory);
            DefaultSsChunkSource = new DefaultSsChunkSource.Factory(DefaultDataSourceFactory);

            BandwidthMeter = new DefaultBandwidthMeter();
            TrackSelectionFactory = new AdaptiveTrackSelection.Factory(BandwidthMeter);
            TrackSelector = new DefaultTrackSelector(TrackSelectionFactory);
            MediaSource = new ConcatenatingMediaSource();

            //TODO: Replace with built-in AudioManager in Exoplayer 2.9.0
            AudioFocusManager = new AudioFocusManager(this);

            Player = ExoPlayerFactory.NewSimpleInstance(Context, TrackSelector);
            Player.AudioAttributes.ContentType = AudioAttributesContentType;
            Player.AudioAttributes.Usage = AudioAttributesUsage;

            PlayerEventListener = new PlayerEventListener()
            {
                OnPlayerErrorImpl = (exception) =>
                {
                    MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, exception, exception.Message));
                },
                OnTracksChangedImpl = (trackGroups, trackSelections) =>
                {
                    MediaManager.MediaQueue.CurrentIndex = Player.CurrentWindowIndex;
                }
            };
            Player.AddListener(PlayerEventListener);

            PlaybackController = new PlaybackController(AudioFocusManager);
            MediaSessionConnector = new MediaSessionConnector(MediaSession, PlaybackController);

            QueueNavigator = new QueueNavigator(MediaSession);
            MediaSessionConnector.SetQueueNavigator(QueueNavigator);

            QueueDataAdapter = new QueueDataAdapter(MediaSource);
            MediaSourceFactory = new MediaSourceFactory();
            TimelineQueueEditor = new TimelineQueueEditor(MediaSession.Controller, MediaSource, QueueDataAdapter, MediaSourceFactory);
            MediaSessionConnector.SetQueueEditor(TimelineQueueEditor);

            RatingCallback = new RatingCallback();
            MediaSessionConnector.SetRatingCallback(RatingCallback);

            PlaybackPreparer = new MediaSessionConnectorPlaybackPreparer(Player, MediaSource);
            MediaSessionConnector.SetPlayer(Player, PlaybackPreparer, null);
        }

        public async Task Play(IMediaItem mediaItem)
        {
            MediaSource.Clear();
            MediaSource.AddMediaSource(mediaItem.ToMediaSource());
            Player.Prepare(MediaSource);
            await Play();
        }

        public Task Play()
        {
            Player.PlayWhenReady = true;
            return Task.CompletedTask;
        }

        public Task Pause()
        {
            Player.Stop();
            return Task.CompletedTask;
        }

        public Task Seek(TimeSpan position)
        {
            Player.SeekTo(position.Milliseconds);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            Player.Stop();
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            Player.Release();
            Player = null;

            base.Dispose(disposing);
        }
    }
}

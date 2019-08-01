using System;
using System.Collections.Generic;
using System.Linq;
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
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using MediaManager.Media;
using MediaManager.Platforms.Android.Media;
using MediaManager.Platforms.Android.MediaSession;
using MediaManager.Platforms.Android.Queue;
using MediaManager.Platforms.Android.Video;
using MediaManager.Player;
using MediaManager.Video;

namespace MediaManager.Platforms.Android.Player
{
    public class AndroidMediaPlayer : Java.Lang.Object, IMediaPlayer<SimpleExoPlayer, VideoView>
    {
        public AndroidMediaPlayer()
        {
        }

        protected AndroidMediaPlayer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        protected Dictionary<string, string> RequestHeaders => MediaManager.RequestHeaders;

        public bool AutoAttachVideoView { get; set; } = true;

        public VideoAspectMode VideoAspect { get; set; }
        public bool ShowPlaybackControls { get; set; } = true;

        public int VideoHeight => 0;
        public int VideoWidth => 0;

        protected Context Context => MediaManager.Context;

        protected string UserAgent { get; set; }
        protected DefaultHttpDataSourceFactory HttpDataSourceFactory { get; set; }
        public IDataSourceFactory DataSourceFactory { get; set; }
        public DefaultDashChunkSource.Factory DashChunkSourceFactory { get; set; }
        public DefaultSsChunkSource.Factory SsChunkSourceFactory { get; set; }

        protected DefaultBandwidthMeter BandwidthMeter { get; set; }
        protected AdaptiveTrackSelection.Factory TrackSelectionFactory { get; set; }
        protected DefaultTrackSelector TrackSelector { get; set; }
        protected PlaybackController PlaybackController { get; set; }

        protected MediaSessionConnector MediaSessionConnector { get; set; }
        protected QueueNavigator QueueNavigator { get; set; }
        protected ConcatenatingMediaSource MediaSource { get; set; }
        protected QueueDataAdapter QueueDataAdapter { get; set; }
        protected QueueMediaSourceFactory MediaSourceFactory { get; set; }
        protected TimelineQueueEditor TimelineQueueEditor { get; set; }
        protected MediaSessionConnectorPlaybackPreparer PlaybackPreparer { get; set; }
        public PlayerEventListener PlayerEventListener { get; set; }
        protected RatingCallback RatingCallback { get; set; }

        private SimpleExoPlayer _player;
        public SimpleExoPlayer Player
        {
            get
            {
                if (_player == null)
                    Initialize();
                return _player;
            }

            set => _player = value;
        }

        public VideoView PlayerView => VideoView as VideoView;

        private IVideoView _videoView;
        public IVideoView VideoView
        {
            get => _videoView;
            set
            {
                _videoView = value;
                if (PlayerView != null)
                {
                    PlayerView.RequestFocus();

                    //Use private field to prevent calling Initialize here
                    if (_player != null)
                        PlayerView.Player = Player;
                }
            }
        }

        protected int lastWindowIndex = 0;

        public MediaSessionCompat MediaSession => MediaManager.MediaSession;

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        protected virtual void Initialize()
        {
            if (RequestHeaders?.Count > 0 && RequestHeaders.TryGetValue("User-Agent", out var userAgent))
                UserAgent = userAgent;
            else
                UserAgent = Util.GetUserAgent(Context, Context.PackageName);

            HttpDataSourceFactory = new DefaultHttpDataSourceFactory(UserAgent);
            UpdateRequestHeaders();

            MediaSource = new ConcatenatingMediaSource();

            DataSourceFactory = new DefaultDataSourceFactory(Context, HttpDataSourceFactory);
            DashChunkSourceFactory = new DefaultDashChunkSource.Factory(DataSourceFactory);
            SsChunkSourceFactory = new DefaultSsChunkSource.Factory(DataSourceFactory);

            Player = ExoPlayerFactory.NewSimpleInstance(Context);

            var audioAttributes = new Com.Google.Android.Exoplayer2.Audio.AudioAttributes.Builder()
             .SetUsage(C.UsageMedia)
             .SetContentType(C.ContentTypeMusic)
             .Build();

            Player.SetAudioAttributes(audioAttributes, true);

            PlayerEventListener = new PlayerEventListener()
            {
                OnPlayerErrorImpl = (ExoPlaybackException exception) =>
                {
                    switch (exception.Type)
                    {
                        case ExoPlaybackException.TypeRenderer:
                        case ExoPlaybackException.TypeSource:
                        case ExoPlaybackException.TypeUnexpected:
                            break;
                    }
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
                OnPlayerStateChangedImpl = (bool playWhenReady, int playbackState) =>
                {
                    switch (playbackState)
                    {
                        case Com.Google.Android.Exoplayer2.Player.StateEnded:
                            if (!Player.HasNext)
                                MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));
                            //TODO: This means the whole list is finished. Should we fire an event?
                            break;
                        case Com.Google.Android.Exoplayer2.Player.StateIdle:
                        case Com.Google.Android.Exoplayer2.Player.StateBuffering:
                            //MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferedPosition);
                            break;
                        case Com.Google.Android.Exoplayer2.Player.StateReady:
                        default:
                            break;
                    }
                },
                OnPositionDiscontinuityImpl = (int reason) =>
                {
                    switch (reason)
                    {
                        case Com.Google.Android.Exoplayer2.Player.DiscontinuityReasonAdInsertion:
                        case Com.Google.Android.Exoplayer2.Player.DiscontinuityReasonSeek:
                        case Com.Google.Android.Exoplayer2.Player.DiscontinuityReasonSeekAdjustment:
                            break;
                        case Com.Google.Android.Exoplayer2.Player.DiscontinuityReasonPeriodTransition:
                            var currentWindowIndex = Player.CurrentWindowIndex;
                            /*if (lastWindowIndex == currentWindowIndex - 1)
                            {
                                // skipped to next
                            }
                            else if (lastWindowIndex == currentWindowIndex + 1)
                            {
                                // skipped to previous
                            }
                            else
                            {
                                // jumped more than one window index
                            }*/
                            if (currentWindowIndex != lastWindowIndex)
                            {
                                MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.ElementAtOrDefault(lastWindowIndex)));
                                lastWindowIndex = currentWindowIndex;
                            }
                            break;
                        case Com.Google.Android.Exoplayer2.Player.DiscontinuityReasonInternal:
                            break;
                    }
                },
                OnLoadingChangedImpl = (bool isLoading) =>
                {
                    if (isLoading)
                        MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferedPosition);
                }
            };
            Player.AddListener(PlayerEventListener);

            ConnectMediaSession();

            if (PlayerView != null && PlayerView.Player == null)
                PlayerView.Player = Player;
        }

        public virtual void UpdateRequestHeaders()
        {
            if (RequestHeaders?.Count > 0)
            {
                foreach (var item in RequestHeaders)
                {
                    HttpDataSourceFactory?.DefaultRequestProperties.Set(item.Key, item.Value);
                }
            }
        }

        public virtual void ConnectMediaSession()
        {
            if (MediaSession == null)
                throw new ArgumentNullException(nameof(MediaSession), $"{nameof(MediaSession)} cannot be null. Make sure the {nameof(MediaBrowserService)} sets it up");

            PlaybackController = new PlaybackController();
            MediaSessionConnector = new MediaSessionConnector(MediaSession, PlaybackController);

            QueueNavigator = new QueueNavigator(MediaSession);
            MediaSessionConnector.SetQueueNavigator(QueueNavigator);

            QueueDataAdapter = new QueueDataAdapter(MediaSource);
            MediaSourceFactory = new QueueMediaSourceFactory();
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
            Player.PlayWhenReady = true;
            return Task.CompletedTask;
        }

        public Task Pause()
        {
            Player.Stop();
            return Task.CompletedTask;
        }

        public Task SeekTo(TimeSpan position)
        {
            Player.SeekTo((long)position.TotalMilliseconds);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            Player.Stop();
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            if (Player != null)
            {
                Player.RemoveListener(PlayerEventListener);
                Player.Release();
                Player = null;
            }

            base.Dispose(disposing);
        }
    }
}

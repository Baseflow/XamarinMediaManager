using System.Windows;
using System.Windows.Controls;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Platforms.Wpf.Video;
using MediaManager.Player;
using MediaManager.Video;

namespace MediaManager.Platforms.Wpf.Player
{
    public class WpfMediaPlayer : MediaPlayerBase, IMediaPlayer<MediaElement, VideoView>
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

        public WpfMediaPlayer()
        {
        }

        public VideoView PlayerView => VideoView as VideoView;

        private IVideoView _videoView;
        public override IVideoView VideoView
        {
            get => _videoView;
            set
            {
                SetProperty(ref _videoView, value);
                UpdateVideoView();
            }
        }

        private MediaElement _player;
        public MediaElement Player
        {
            get
            {
                if (_player == null)
                    Initialize();
                return _player;
            }
            set => SetProperty(ref _player, value);
        }

        public override void UpdateVideoAspect(VideoAspectMode videoAspectMode)
        {
            if (PlayerView == null)
                return;

            var playerView = Player;

            switch (videoAspectMode)
            {
                case VideoAspectMode.None:
                    playerView.Stretch = System.Windows.Media.Stretch.None;
                    break;
                case VideoAspectMode.AspectFit:
                    playerView.Stretch = System.Windows.Media.Stretch.UniformToFill;
                    break;
                case VideoAspectMode.AspectFill:
                    playerView.Stretch = System.Windows.Media.Stretch.Fill;
                    break;
                default:
                    playerView.Stretch = System.Windows.Media.Stretch.None;
                    break;
            }
        }

        public override void UpdateShowPlaybackControls(bool showPlaybackControls)
        {
            if (PlayerView == null)
                return;

            //Player. = showPlaybackControls;
        }

        public override void UpdateVideoPlaceholder(object value)
        {
            if (PlayerView == null)
                return;

            //TODO: Implement placeholder
        }

        public override void UpdateIsFullWindow(bool isFullWindow)
        {
            if (PlayerView == null)
                return;

            //TODO: Implement isFullWindow
        }

        public virtual void Initialize()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Player = new MediaElement
                {
                    LoadedBehavior = MediaState.Play,
                    UnloadedBehavior = MediaState.Manual,
                    Volume = 1,
                    IsMuted = false
                };

                Player.MediaEnded += Player_MediaEnded;
                Player.MediaOpened += Player_MediaOpened;
                Player.MediaFailed += Player_MediaFailed;
                Player.BufferingStarted += Player_BufferingStarted;
                Player.BufferingEnded += Player_BufferingEnded;
            });
        }

        protected virtual void Player_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            MediaManager.State = MediaPlayerState.Failed;
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.Queue.Current, e.ErrorException, e.ErrorException.Message));
        }

        protected virtual void Player_BufferingEnded(object sender, EventArgs e)
        {
            MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferingProgress);
        }

        protected virtual void Player_BufferingStarted(object sender, EventArgs e)
        {
            MediaManager.State = MediaPlayerState.Buffering;
            MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferingProgress);
        }

        protected virtual void Player_MediaOpened(object sender, EventArgs e)
        {
            MediaManager.State = MediaPlayerState.Playing;
        }

        protected virtual void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.Queue.Current));
        }

        public override Task Pause()
        {
            Player.Pause();
            MediaManager.State = MediaPlayerState.Paused;
            return Task.CompletedTask;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            InvokeBeforePlaying(this, new MediaPlayerEventArgs(mediaItem, this));

            await Play(new Uri(mediaItem.MediaUri));

            InvokeAfterPlaying(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public override async Task Play(IMediaItem mediaItem, TimeSpan startAt, TimeSpan? stopAt = null)
        {
            InvokeBeforePlaying(this, new MediaPlayerEventArgs(mediaItem, this));

            await Play(new Uri(mediaItem.MediaUri));

            if (startAt != TimeSpan.Zero)
                await SeekTo(startAt);

            InvokeAfterPlaying(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public virtual async Task Play(Uri uri)
        {
            try
            {
                Player.Source = uri;
                await Play();
            }
            catch (Exception ex)
            {
                MediaManager.State = MediaPlayerState.Failed;
                MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.Queue.Current, ex, ex.Message));
            }
        }

        public override Task Play()
        {
            Player.Play();
            MediaManager.State = MediaPlayerState.Playing;
            return Task.CompletedTask;
        }

        public override Task SeekTo(TimeSpan position)
        {
            Player.Position = position;
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            Player.Pause();
            MediaManager.State = MediaPlayerState.Stopped;
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            Player.MediaEnded -= Player_MediaEnded;
            Player.MediaOpened -= Player_MediaOpened;
            Player.MediaFailed -= Player_MediaFailed;
            Player.BufferingStarted -= Player_BufferingStarted;
            Player.BufferingEnded -= Player_BufferingEnded;
            Player = null;
        }
    }
}

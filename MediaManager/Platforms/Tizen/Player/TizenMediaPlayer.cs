using System;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Platforms.Tizen.Media;
using MediaManager.Platforms.Tizen.Video;
using MediaManager.Player;
using MediaManager.Video;
using Tizen.Multimedia;
using TizenPlayer = Tizen.Multimedia.Player;

namespace MediaManager.Platforms.Tizen.Player
{
    public class TizenMediaPlayer : MediaPlayerBase, IMediaPlayer<TizenPlayer, VideoView>
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Tizen;

        public TizenMediaPlayer()
        {
        }

        private TizenPlayer _player;
        public TizenPlayer Player
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

            //TODO: Set on player
        }

        public override void UpdateShowPlaybackControls(bool showPlaybackControls)
        {
            if (PlayerView == null)
                return;

            //TODO
        }

        public override void UpdateVideoPlaceholder(object value)
        {
            if (PlayerView == null)
                return;

            //TODO: Implement placeholder
        }

        protected virtual void Initialize()
        {
            Player = new TizenPlayer();
            Player.ErrorOccurred += Player_ErrorOccurred;
            Player.PlaybackInterrupted += Player_PlaybackInterrupted;
            Player.PlaybackCompleted += Player_PlaybackCompleted;
            Player.BufferingProgressChanged += Player_BufferingProgressChanged;
        }

        private void Player_BufferingProgressChanged(object sender, BufferingProgressChangedEventArgs e)
        {
            //TODO: Percent is not correct here
            MediaManager.Buffered = TimeSpan.FromMilliseconds(e.Percent);
        }

        private void Player_PlaybackCompleted(object sender, EventArgs e)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.Queue.Current));
        }

        private void Player_PlaybackInterrupted(object sender, PlaybackInterruptedEventArgs e)
        {

        }

        private void Player_ErrorOccurred(object sender, PlayerErrorOccurredEventArgs e)
        {
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.Queue.Current, new Exception(e.ToString()), e.ToString()));
        }

        public override IVideoView VideoView { get; set; }

        public VideoView PlayerView => VideoView as VideoView;

        public override event BeforePlayingEventHandler BeforePlaying;
        public override event AfterPlayingEventHandler AfterPlaying;

        public override Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
            await Play(mediaItem.ToMediaSource());
            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public override async Task Play(IMediaItem mediaItem, TimeSpan startAt, TimeSpan? stopAt = null)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));

            //TODO: Implement stopAt

            await Play(mediaItem.ToMediaSource());

            if (startAt != TimeSpan.Zero)
                await SeekTo(startAt);

            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public virtual async Task Play(MediaSource mediaSource)
        {
            Player.SetSource(mediaSource);
            await Player.PrepareAsync();
            Player.Start();
        }

        public override Task Play()
        {
            Player.Start();
            return Task.CompletedTask;
        }

        public override async Task SeekTo(TimeSpan position)
        {
            //TODO: Probably not good
            await Player.SetPlayPositionAsync(Convert.ToInt32(position.TotalMilliseconds), false);
        }

        public override Task Stop()
        {
            Player.Stop();
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            Player.ErrorOccurred -= Player_ErrorOccurred;
            Player.PlaybackInterrupted -= Player_PlaybackInterrupted;
            Player.PlaybackCompleted -= Player_PlaybackCompleted;
            Player.BufferingProgressChanged -= Player_BufferingProgressChanged;
        }
    }
}

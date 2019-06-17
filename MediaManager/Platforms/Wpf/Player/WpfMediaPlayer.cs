using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MediaManager.Media;
using MediaManager.Platforms.Wpf.Video;
using MediaManager.Playback;
using MediaManager.Video;

namespace MediaManager.Platforms.Wpf.Player
{
    public class WpfMediaPlayer : IMediaPlayer<MediaPlayer, VideoView>
    {
        public WpfMediaPlayer()
        {
        }

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

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
                    PlayerView.PlayerView = this.PlayerView.PlayerView;
                }
            }
        }

        private MediaPlayer _player;
        public MediaPlayer Player
        {
            get
            {
                if (_player == null)
                    Initialize();
                return _player;
            }
            set
            {
                _player = value;
            }
        }
        public RepeatMode RepeatMode { get; set; }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public void Initialize()
        {
            Player = new MediaPlayer();
            Player.MediaEnded += Player_MediaEnded;
            Player.MediaOpened += Player_MediaOpened;
            Player.BufferingStarted += Player_BufferingStarted;
            Player.BufferingEnded += Player_BufferingEnded;
            Player.MediaFailed += Player_MediaFailed;
        }

        private void Player_MediaFailed(object sender, ExceptionEventArgs e)
        {
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, e.ErrorException, e.ErrorException.Message));
        }

        private void Player_BufferingEnded(object sender, EventArgs e)
        {
            MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferingProgress);
        }

        private void Player_BufferingStarted(object sender, EventArgs e)
        {
            MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferingProgress);
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));
        }

        public Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public async Task Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));

            Player.Open(new Uri(mediaItem.MediaUri));
            await Play();

            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public Task Play()
        {
            Player.Play();
            return Task.CompletedTask;
        }

        public Task SeekTo(TimeSpan position)
        {
            Player.Position = position;
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Player.MediaEnded -= Player_MediaEnded;
            Player.MediaOpened -= Player_MediaOpened;
            Player.BufferingStarted -= Player_BufferingStarted;
            Player.BufferingEnded -= Player_BufferingEnded;
            Player.MediaFailed -= Player_MediaFailed;
            Player = null;
        }
    }
}

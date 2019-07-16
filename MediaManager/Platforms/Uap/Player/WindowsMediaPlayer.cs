using System;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Platforms.Uap.Player;
using MediaManager.Platforms.Uap.Video;
using MediaManager.Playback;
using MediaManager.Video;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace MediaManager.Platforms.Uap.Media
{
    public class WindowsMediaPlayer : IMediaPlayer<MediaPlayer, VideoView>
    {
        public WindowsMediaPlayer()
        {
        }

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Windows;

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
                    PlayerView.PlayerView.SetMediaPlayer(Player);
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

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        private MediaPlaybackList _mediaPlaybackList;
        public MediaPlaybackList MediaPlaybackList
        {
            get
            {
                if(_mediaPlaybackList == null)
                    _mediaPlaybackList = new MediaPlaybackList();
                return _mediaPlaybackList;
            }
            private set => _mediaPlaybackList = value;
        }

        public void Initialize()
        {
            Player = new MediaPlayer();
            Player.AudioCategory = MediaPlayerAudioCategory.Media;

            Player.MediaEnded += Player_MediaEnded;
            Player.MediaFailed += Player_MediaFailed;
            Player.PlaybackSession.BufferingProgressChanged += PlaybackSession_BufferingProgressChanged;
            Player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            Player.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
        }

        private void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
            //TODO: Maybe use this?
        }

        private void PlaybackSession_BufferingProgressChanged(MediaPlaybackSession sender, object args)
        {
            MediaManager.Buffered = TimeSpan.FromMilliseconds(sender.BufferingProgress);
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            MediaManager.State = sender.PlaybackState.ToMediaPlayerState();
        }

        private void Player_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, new Exception(args.ErrorMessage), args.ErrorMessage));
        }

        private void Player_MediaEnded(MediaPlayer sender, object args)
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

            var item = new MediaPlaybackItem(mediaItem.ToMediaSource());

            //TODO: Fill MediaPlaybackList with full queue
            MediaPlaybackList.Items.Clear();
            MediaPlaybackList.Items.Add(item);
            Player.Source = MediaPlaybackList;

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
            Player.PlaybackSession.Position = position;
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            Player.Pause();
            await SeekTo(TimeSpan.Zero);
            MediaManager.State = Playback.MediaPlayerState.Stopped;
        }

        public void Dispose()
        {
            Player.MediaEnded -= Player_MediaEnded;
            Player.MediaFailed -= Player_MediaFailed;
            Player.PlaybackSession.BufferingProgressChanged -= PlaybackSession_BufferingProgressChanged;
            Player.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
            Player.PlaybackSession.PositionChanged -= PlaybackSession_PositionChanged;
            Player.Dispose();
            Player = null;
        }
    }
}

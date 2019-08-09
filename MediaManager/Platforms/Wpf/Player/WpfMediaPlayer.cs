﻿using System;
using System.Threading.Tasks;
using System.Windows.Controls;
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
                _videoView = value;
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
            set
            {
                _player = value;
            }
        }

        public override event BeforePlayingEventHandler BeforePlaying;
        public override event AfterPlayingEventHandler AfterPlaying;

        public void Initialize()
        {
            Player = new MediaElement();
            Player.LoadedBehavior = MediaState.Play;
            Player.UnloadedBehavior = MediaState.Manual;
            Player.Volume = 1;
            Player.IsMuted = false;

            Player.MediaEnded += Player_MediaEnded;
            Player.MediaOpened += Player_MediaOpened;
            Player.MediaFailed += Player_MediaFailed;
            Player.BufferingStarted += Player_BufferingStarted;
            Player.BufferingEnded += Player_BufferingEnded;
        }

        private void Player_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            MediaManager.State = MediaPlayerState.Failed;
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, e.ErrorException, e.ErrorException.Message));
        }

        private void Player_BufferingEnded(object sender, EventArgs e)
        {
            MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferingProgress);
        }

        private void Player_BufferingStarted(object sender, EventArgs e)
        {
            MediaManager.State = MediaPlayerState.Buffering;
            MediaManager.Buffered = TimeSpan.FromMilliseconds(Player.BufferingProgress);
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            MediaManager.State = MediaPlayerState.Playing;
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));
        }

        public override Task Pause()
        {
            Player.Pause();
            MediaManager.State = MediaPlayerState.Paused;
            return Task.CompletedTask;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
            try
            {
                Player.Source = new Uri(mediaItem.MediaUri);
                await Play();
            }
            catch (Exception ex)
            {
                MediaManager.State = MediaPlayerState.Failed;
                MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, ex, ex.Message));
            }
            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
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

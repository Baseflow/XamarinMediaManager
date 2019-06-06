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
            Initialize();
        }

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

        public VideoView PlayerView { get; set; }
        public IVideoView VideoView => PlayerView;

        public MediaPlayer Player { get; set; }

        public Playback.MediaPlayerState State => MediaPlayerState.Stopped;

        public RepeatMode RepeatMode { get; set; }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public void Initialize()
        {
            if (Player != null)
                return;

            Player = new MediaPlayer();
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
            Player = null;
        }
    }
}

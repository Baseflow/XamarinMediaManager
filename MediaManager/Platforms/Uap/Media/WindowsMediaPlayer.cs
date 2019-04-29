using System;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;
using Windows.Media.Playback;

namespace MediaManager.Platforms.Uap.Media
{
    public class WindowsMediaPlayer : IMediaPlayer<MediaPlayer, MediaPlayerSurface>
    {
        public MediaPlayer Player { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Playback.MediaPlayerState State => throw new NotImplementedException();

        public MediaPlayerSurface PlayerView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RepeatMode RepeatMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        public Task Play()
        {
            throw new NotImplementedException();
        }

        public Task Seek(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}

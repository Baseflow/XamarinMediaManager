using System;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Media
{
    public class MediaPlayer : IMediaPlayer<Player, MediaView>
    {
        public Player Player { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaPlayerState State => throw new NotImplementedException();

        public MediaView PlayerView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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

        public Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}

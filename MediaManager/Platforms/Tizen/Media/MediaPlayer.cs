using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Media
{
    public class MediaPlayer : IAudioPlayer<Player>, IVideoPlayer<Player>
    {
        public Player Player { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaPlayerState State => throw new NotImplementedException();

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

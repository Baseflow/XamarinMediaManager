using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;

namespace MediaManager.Platforms.Apple.Media
{
    public class AppleMediaPlayer : NSObject, IAudioPlayer, IVideoPlayer
    {
        public AppleMediaPlayer()
        {
        }

        public AVQueuePlayer Player { get; set; }

        public MediaPlayerState State => throw new NotImplementedException();

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public virtual void Initialize()
        {
            Player = new AVQueuePlayer();
        }

        public Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public Task Play(IMediaItem mediaItem)
        {
            var item = mediaItem.GetPlayerItem();

            Player.ReplaceCurrentItemWithPlayerItem(item);
            Player.Play();
            return Task.CompletedTask;
        }

        public Task Play()
        {
            Player.Play();
            return Task.CompletedTask;
        }

        public async Task Seek(TimeSpan position)
        {
            await Player.SeekAsync(CMTime.FromSeconds(position.TotalSeconds, 1));
        }

        public Task Stop()
        {
            Player.Pause();
            return Task.CompletedTask;
        }
    }
}

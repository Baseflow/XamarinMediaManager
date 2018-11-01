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


namespace MediaManager.Platforms.Apple.Media
{
    public abstract class AppleMediaPlayer : NSObject, IAudioPlayer<AVQueuePlayer> //, IVideoPlayer<AVQueuePlayer, UIView>
    {
        private NSObject DidFinishPlayingObserver;
        private NSObject ItemFailedToPlayToEndTimeObserver;
        private NSObject ErrorObserver;

        public AppleMediaPlayer()
        {
        }

        public AVQueuePlayer Player { get; set; }

        public MediaPlayerState State => throw new NotImplementedException();

        //public UIView PlayerView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public virtual void Initialize()
        {
            DidFinishPlayingObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidFinishPlaying);
            ItemFailedToPlayToEndTimeObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidErrorOcurred);
            ErrorObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, DidErrorOcurred);
            Player = new AVQueuePlayer();
        }

        private void DidErrorOcurred(NSNotification obj)
        {
            throw new NotImplementedException();
        }

        private void DidFinishPlaying(NSNotification obj)
        {
            throw new NotImplementedException();
        }

        public Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public Task Play(IMediaItem mediaItem)
        {
            var item = mediaItem.GetPlayerItem();

            Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
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

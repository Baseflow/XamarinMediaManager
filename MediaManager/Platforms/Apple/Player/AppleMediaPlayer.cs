using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using MediaManager.Media;
using MediaManager.Platforms.Apple.Playback;
using MediaManager.Playback;
using MediaManager.Video;

namespace MediaManager.Platforms.Apple.Media
{
    public abstract class AppleMediaPlayer : NSObject, IMediaPlayer<AVQueuePlayer>
    {
        private NSObject DidFinishPlayingObserver;
        private NSObject ItemFailedToPlayToEndTimeObserver;
        private NSObject ErrorObserver;
        private NSObject PlaybackStalledObserver;

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Apple;

        public AppleMediaPlayer()
        {
        }

        public abstract IVideoView VideoView { get; }

        private AVQueuePlayer _player;
        public AVQueuePlayer Player
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
        
        private IDisposable rateToken;
        private object statusToken;
        private IDisposable timeControlStatusToken;
        private IDisposable loadedTimeRangesToken;
        private IDisposable reasonForWaitingToPlayToken;
        private IDisposable playbackLikelyToKeepUpToken;
        private IDisposable playbackBufferFullToken;
        private IDisposable playbackBufferEmptyToken;

        /*private MediaPlayerState _state;
        public MediaPlayerState State
        {
            get { return _state; }
            private set
            {
                _state = value;
                MediaManager.OnStateChanged(this, new StateChangedEventArgs(_state));
            }
        }*/

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        protected virtual void Initialize()
        {
            Player = new AVQueuePlayer();

            //_state = MediaPlayerState.Stopped;

            DidFinishPlayingObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidFinishPlaying);
            ItemFailedToPlayToEndTimeObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidErrorOcurred);
            ErrorObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, DidErrorOcurred);
            PlaybackStalledObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.PlaybackStalledNotification, DidErrorOcurred);

            var options = NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New;
            rateToken = Player.AddObserver("rate", options, RateChanged);
            statusToken = Player.AddObserver("status", options, StatusChanged);
            timeControlStatusToken = Player.AddObserver("timeControlStatus", options, TimeControlStatusChanged);
            loadedTimeRangesToken = Player.AddObserver("currentItem.loadedTimeRanges", options, LoadedTimeRangesChanged);
            reasonForWaitingToPlayToken = Player.AddObserver("reasonForWaitingToPlay", options, ReasonForWaitingToPlayChanged);
            playbackLikelyToKeepUpToken = Player.AddObserver("currentItem.playbackLikelyToKeepUp", options, PlaybackLikelyToKeepUpChanged);
            playbackBufferFullToken = Player.AddObserver("currentItem.playbackBufferFull", options, PlaybackBufferFullChanged);
            playbackBufferEmptyToken = Player.AddObserver("currentItem.playbackBufferEmpty", options, PlaybackBufferEmptyChanged);

            // Watch the buffering status. If it changes, we may have to resume because the playing stopped because of bad network-conditions.
            /*MediaManager.BufferingChanged += (sender, e) =>
            {
                // If the player is ready to play, it's paused and the status is still on PLAYING, go on!
                if ((Player.Status == AVPlayerStatus.ReadyToPlay) && (Player.Rate == 0.0f) &&
                    (State == MediaPlayerState.Playing))
                    Player.Play();
            };*/
        }

        private void StatusChanged(NSObservedChange obj)
        {
            MediaManager.State = Player.Status.ToMediaPlayerState();
            var state = Player.TimeControlStatus.ToMediaPlayerState();
        }

        private void PlaybackBufferEmptyChanged(NSObservedChange obj)
        {

        }

        private void PlaybackBufferFullChanged(NSObservedChange obj)
        {

        }

        private void PlaybackLikelyToKeepUpChanged(NSObservedChange obj)
        {

        }

        private void ReasonForWaitingToPlayChanged(NSObservedChange obj)
        {

        }

        private void LoadedTimeRangesChanged(NSObservedChange obj)
        {
            var buffered = TimeSpan.Zero;
            if (Player?.CurrentItem != null && Player.CurrentItem.LoadedTimeRanges.Any())
            {
                buffered =
                    TimeSpan.FromSeconds(
                        Player.CurrentItem.LoadedTimeRanges.Select(
                            tr => tr.CMTimeRangeValue.Start.Seconds + tr.CMTimeRangeValue.Duration.Seconds).Max());

                MediaManager.OnBufferingChanged(this, new BufferingChangedEventArgs(buffered));
            }
        }

        private void RateChanged(NSObservedChange obj)
        {

        }

        private void TimeControlStatusChanged(NSObservedChange obj)
        {
            if(Player.Status != AVPlayerStatus.Unknown)
                MediaManager.State = Player.TimeControlStatus.ToMediaPlayerState();

            //var state = Player.Status.ToMediaPlayerState();
        }

        private void DidErrorOcurred(NSNotification obj)
        {
            var error = Player?.CurrentItem?.Error;
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue?.Current, new NSErrorException(error), error?.LocalizedDescription));
        }

        private async void DidFinishPlaying(NSNotification obj)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));

            //TODO: Android has its own queue and goes to next. Maybe use native apple queue
            var succesfullNext = await MediaManager.PlayNext();
            if (succesfullNext)
                MediaManager.OnMediaItemChanged(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));
            else
                await Stop();
        }

        public Task Pause()
        {
            Player.Pause();
            //State = MediaPlayerState.Stopped;
            return Task.CompletedTask;
        }

        public Task Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));

            var item = mediaItem.ToAVPlayerItem();

            Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
            Player.ReplaceCurrentItemWithPlayerItem(item);
            Player.Play();

            //State = MediaPlayerState.Playing;
            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
            return Task.CompletedTask;
        }

        public Task Play()
        {
            Player.Play();
            //State = MediaPlayerState.Playing;
            return Task.CompletedTask;
        }

        public async Task SeekTo(TimeSpan position)
        {
            await Player.SeekAsync(CMTime.FromSeconds(position.TotalSeconds, 1));
        }

        public Task Stop()
        {
            Player.Pause();
            //State = MediaPlayerState.Stopped;
            return Task.CompletedTask;
        }

        public RepeatMode RepeatMode { get; set; } = RepeatMode.Off;

        protected override void Dispose(bool disposing)
        {
            NSNotificationCenter.DefaultCenter.RemoveObservers(new List<NSObject>(){
                DidFinishPlayingObserver,
                ItemFailedToPlayToEndTimeObserver,
                ErrorObserver,
                PlaybackStalledObserver
            });

            base.Dispose(disposing);
        }
    }
}

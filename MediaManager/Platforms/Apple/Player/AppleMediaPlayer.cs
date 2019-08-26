using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Platforms.Apple.Playback;
using MediaManager.Playback;
using MediaManager.Player;

namespace MediaManager.Platforms.Apple.Player
{
    public abstract class AppleMediaPlayer : MediaPlayerBase, IMediaPlayer<AVQueuePlayer>
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Apple;

        public AppleMediaPlayer()
        {
        }

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

        #region RepeatMode

        private RepeatMode _repeatMode;
        public virtual RepeatMode RepeatMode
        {
            get => _repeatMode;
            set => SetProperty(ref _repeatMode, value);
        }

        #endregion

        private NSObject didFinishPlayingObserver;
        private NSObject itemFailedToPlayToEndTimeObserver;
        private NSObject errorObserver;
        private NSObject playbackStalledObserver;

        private IDisposable rateToken;
        private IDisposable statusToken;
        private IDisposable timeControlStatusToken;
        private IDisposable loadedTimeRangesToken;
        private IDisposable reasonForWaitingToPlayToken;
        private IDisposable playbackLikelyToKeepUpToken;
        private IDisposable playbackBufferFullToken;
        private IDisposable playbackBufferEmptyToken;
        private IDisposable presentationSizeToken;

        public override event BeforePlayingEventHandler BeforePlaying;
        public override event AfterPlayingEventHandler AfterPlaying;

        protected virtual void Initialize()
        {
            Player = new AVQueuePlayer();

            didFinishPlayingObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidFinishPlaying);
            itemFailedToPlayToEndTimeObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidErrorOcurred);
            errorObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, DidErrorOcurred);
            playbackStalledObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.PlaybackStalledNotification, DidErrorOcurred);

            var options = NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New;
            rateToken = Player.AddObserver("rate", options, RateChanged);
            statusToken = Player.AddObserver("status", options, StatusChanged);
            timeControlStatusToken = Player.AddObserver("timeControlStatus", options, TimeControlStatusChanged);
            reasonForWaitingToPlayToken = Player.AddObserver("reasonForWaitingToPlay", options, ReasonForWaitingToPlayChanged);

            loadedTimeRangesToken = Player.AddObserver("currentItem.loadedTimeRanges", options, LoadedTimeRangesChanged);
            playbackLikelyToKeepUpToken = Player.AddObserver("currentItem.playbackLikelyToKeepUp", options, PlaybackLikelyToKeepUpChanged);
            playbackBufferFullToken = Player.AddObserver("currentItem.playbackBufferFull", options, PlaybackBufferFullChanged);
            playbackBufferEmptyToken = Player.AddObserver("currentItem.playbackBufferEmpty", options, PlaybackBufferEmptyChanged);
            presentationSizeToken = Player.AddObserver("currentItem.presentationSize", options, PresentationSizeChanged);
        }

        private void PresentationSizeChanged(NSObservedChange obj)
        {
            if (Player.CurrentItem != null && !Player.CurrentItem.PresentationSize.IsEmpty)
            {
                VideoWidth = (int)Player.CurrentItem.PresentationSize.Width;
                VideoHeight = (int)Player.CurrentItem.PresentationSize.Height;
            }
        }

        private void StatusChanged(NSObservedChange obj)
        {
            MediaManager.State = Player.Status.ToMediaPlayerState();
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
            var reason = Player.ReasonForWaitingToPlay;
            if (reason == null)
            {
            }
            else if (reason == AVPlayer.WaitingToMinimizeStallsReason)
            {
            }
            else if (reason == AVPlayer.WaitingWhileEvaluatingBufferingRateReason)
            {
            }
            else if (reason == AVPlayer.WaitingWithNoItemToPlayReason)
            {
            }
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

                MediaManager.Buffered = buffered;
            }
        }

        private void RateChanged(NSObservedChange obj)
        {
            //TODO: Maybe set the rate from here
        }

        private void TimeControlStatusChanged(NSObservedChange obj)
        {
            if (Player.Status != AVPlayerStatus.Unknown)
                MediaManager.State = Player.TimeControlStatus.ToMediaPlayerState();
        }

        private void DidErrorOcurred(NSNotification obj)
        {
            //TODO: Error should not be null after this or it will crash.
            var error = Player?.CurrentItem?.Error ?? new NSError();
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue?.Current, new NSErrorException(error), error?.LocalizedDescription));
        }

        private async void DidFinishPlaying(NSNotification obj)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));

            //TODO: Android has its own queue and goes to next. Maybe use native apple queue
            var succesfullNext = await MediaManager.PlayNext();
            if (!succesfullNext)
            {
                await Stop();
            }
            else if (RepeatMode == RepeatMode.All && MediaManager.MediaQueue.Any())
            {
                MediaManager.MediaQueue.CurrentIndex = 0;
                await MediaManager.PlayQueueItem(MediaManager.MediaQueue.Current);
            }
            else if (RepeatMode == RepeatMode.One && MediaManager.MediaQueue.Any())
            {
                MediaManager.MediaQueue.CurrentIndex = MediaManager.MediaQueue.CurrentIndex - 1;
                await MediaManager.PlayQueueItem(MediaManager.MediaQueue.Current);
            }
        }

        public override Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            BeforePlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));

            var item = mediaItem.ToAVPlayerItem();

            Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
            Player.ReplaceCurrentItemWithPlayerItem(item);
            MediaManager.OnMediaItemChanged(this, new MediaItemEventArgs(mediaItem));
            await Play();

            AfterPlaying?.Invoke(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public override Task Play()
        {
            Player.Play();
            return Task.CompletedTask;
        }

        public override async Task SeekTo(TimeSpan position)
        {
            await Player.SeekAsync(CMTime.FromSeconds(position.TotalSeconds, 1));
        }

        public override async Task Stop()
        {
            Player.Pause();
            await SeekTo(TimeSpan.Zero);
            MediaManager.State = MediaPlayerState.Stopped;
        }

        protected override void Dispose(bool disposing)
        {
            NSNotificationCenter.DefaultCenter.RemoveObservers(new List<NSObject>(){
                didFinishPlayingObserver,
                itemFailedToPlayToEndTimeObserver,
                errorObserver,
                playbackStalledObserver
            });

            rateToken?.Dispose();
            statusToken?.Dispose();
            timeControlStatusToken?.Dispose();
            reasonForWaitingToPlayToken?.Dispose();
            playbackLikelyToKeepUpToken?.Dispose();
            loadedTimeRangesToken?.Dispose();
            playbackBufferFullToken?.Dispose();
            playbackBufferEmptyToken?.Dispose();
            presentationSizeToken?.Dispose();
        }
    }
}

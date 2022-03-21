using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Apple.Playback;
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
            set => SetProperty(ref _player, value);
        }

        public int TimeScale { get; set; } = 60;

        private NSObject didFinishPlayingObserver;
        private NSObject itemFailedToPlayToEndTimeObserver;
        private NSObject errorObserver;
        private NSObject playbackStalledObserver;
        private NSObject playbackTimeObserver;

        private IDisposable rateToken;
        private IDisposable statusToken;
        private IDisposable timeControlStatusToken;
        private IDisposable loadedTimeRangesToken;
        private IDisposable reasonForWaitingToPlayToken;
        private IDisposable playbackLikelyToKeepUpToken;
        private IDisposable playbackBufferFullToken;
        private IDisposable playbackBufferEmptyToken;
        private IDisposable presentationSizeToken;
        private IDisposable timedMetaDataToken;

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
            timedMetaDataToken = Player.AddObserver("currentItem.timedMetadata", options, TimedMetaDataChanged);
        }

        protected virtual void PresentationSizeChanged(NSObservedChange obj)
        {
            if (Player.CurrentItem != null && !Player.CurrentItem.PresentationSize.IsEmpty)
            {
                VideoWidth = (int)Player.CurrentItem.PresentationSize.Width;
                VideoHeight = (int)Player.CurrentItem.PresentationSize.Height;
            }
        }

        protected virtual void TimedMetaDataChanged(NSObservedChange obj)
        {
            if (MediaManager.Queue.Current == null || MediaManager.Queue.Current.IsMetadataExtracted)
                return;

            if (obj.NewValue is NSArray array && array.Count > 0)
            {
                var avMetadataItem = array.GetItem<AVMetadataItem>(0);
                if (avMetadataItem != null && !string.IsNullOrEmpty(avMetadataItem.StringValue))
                {
                    var split = avMetadataItem.StringValue.Split(" - ");
                    MediaManager.Queue.Current.Artist = split.FirstOrDefault();

                    if (split.Length > 1)
                    {
                        MediaManager.Queue.Current.Title = split.LastOrDefault();
                    }
                }
            }
        }

        protected virtual void StatusChanged(NSObservedChange obj)
        {
            MediaManager.State = Player.Status.ToMediaPlayerState();
        }

        protected virtual void PlaybackBufferEmptyChanged(NSObservedChange obj)
        {
        }

        protected virtual void PlaybackBufferFullChanged(NSObservedChange obj)
        {
        }

        protected virtual void PlaybackLikelyToKeepUpChanged(NSObservedChange obj)
        {
        }

        protected virtual void ReasonForWaitingToPlayChanged(NSObservedChange obj)
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

        protected virtual void LoadedTimeRangesChanged(NSObservedChange obj)
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

        protected virtual void RateChanged(NSObservedChange obj)
        {
            //TODO: Maybe set the rate from here
        }

        protected virtual void TimeControlStatusChanged(NSObservedChange obj)
        {
            if (Player.Status != AVPlayerStatus.Unknown)
                MediaManager.State = Player.TimeControlStatus.ToMediaPlayerState();
        }

        protected virtual void DidErrorOcurred(NSNotification obj)
        {
            Exception exception = null;
            string message = null;
            
            var error = Player?.CurrentItem?.Error;
            if(error != null)
            {
                exception = new NSErrorException(error);
                message = error.LocalizedDescription;
            }
            else
            {
                message = obj?.ToString() ?? "MediaItem failed with unknown reason";
                exception = new ApplicationException(message);
            }            

            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.Queue?.Current, exception, message));
        }

        protected virtual async void DidFinishPlaying(NSNotification obj)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.Queue.Current));

            //TODO: Android has its own queue and goes to next. Maybe use native apple queue
            var succesfullNext = await MediaManager.PlayNext();
            if (!succesfullNext)
            {
                await Stop();
            }
        }

        public override Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            InvokeBeforePlaying(this, new MediaPlayerEventArgs(mediaItem, this));
            await Play(mediaItem.ToAVPlayerItem());
            InvokeAfterPlaying(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        public override async Task Play(IMediaItem mediaItem, TimeSpan startAt, TimeSpan? stopAt = null)
        {
            InvokeBeforePlaying(this, new MediaPlayerEventArgs(mediaItem, this));

            if (stopAt is TimeSpan endTime)
            {
                var values = new NSValue[]
                {
                NSValue.FromCMTime(CMTime.FromSeconds(endTime.TotalSeconds, TimeScale))
                };

                playbackTimeObserver = Player.AddBoundaryTimeObserver(values, null, OnPlayerBoundaryReached);
            }

            await Play(mediaItem.ToAVPlayerItem());

            if (startAt != TimeSpan.Zero)
                await SeekTo(startAt);

            InvokeAfterPlaying(this, new MediaPlayerEventArgs(mediaItem, this));
        }

        protected virtual async void OnPlayerBoundaryReached()
        {
            await Pause();
            Player.RemoveTimeObserver(playbackTimeObserver);
        }

        public virtual async Task Play(AVPlayerItem playerItem)
        {
            Player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
            Player.RemoveAllItems();
            Player.InsertItem(playerItem, null);
            await Play();
        }

        public override Task Play()
        {
            Player.Play();
            return Task.CompletedTask;
        }

        public override async Task SeekTo(TimeSpan position)
        {
            var scale = TimeScale;

            if (Player?.CurrentItem?.Duration != null && Player?.CurrentItem?.Duration != CMTime.Indefinite)
                scale = Player.CurrentItem.Duration.TimeScale;

            await Player?.SeekAsync(CMTime.FromSeconds(position.TotalSeconds, scale), CMTime.Zero, CMTime.Zero);
        }

        public override async Task Stop()
        {
            if (Player != null)
            {
                Player.Pause();
                await SeekTo(TimeSpan.Zero);
                Player.ReplaceCurrentItemWithPlayerItem(null); // Needed for stop buffering
            }
            if (MediaManager != null)
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

            if (playbackTimeObserver != null)
                Player.RemoveTimeObserver(playbackTimeObserver);

            rateToken?.Dispose();
            statusToken?.Dispose();
            timeControlStatusToken?.Dispose();
            reasonForWaitingToPlayToken?.Dispose();
            playbackLikelyToKeepUpToken?.Dispose();
            loadedTimeRangesToken?.Dispose();
            playbackBufferFullToken?.Dispose();
            playbackBufferEmptyToken?.Dispose();
            presentationSizeToken?.Dispose();
            timedMetaDataToken?.Dispose();
        }
    }
}

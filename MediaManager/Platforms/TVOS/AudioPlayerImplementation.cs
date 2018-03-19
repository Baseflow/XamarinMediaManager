using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class AudioPlayerImplementation : NSObject, IAudioPlayer
    {
        private readonly IVolumeManager _volumeManager;

        private readonly IVersionHelper _versionHelper;

        private IMediaFile _currentMediaFile;

        public static readonly NSString StatusObservationContext = new NSString("Status");

        public static readonly NSString RateObservationContext = new NSString("Rate");

        public static readonly NSString LoadedTimeRangesObservationContext = new NSString("TimeRanges");

        private AVPlayer _player;
        private MediaPlayerStatus _status;

        public Dictionary<string, string> RequestHeaders { get; set; }

        public AudioPlayerImplementation(IVolumeManager volumeManager)
        {
            _volumeManager = volumeManager;
            _versionHelper = new VersionHelper();

            _status = MediaPlayerStatus.Stopped;

            InitializePlayer();

            // Watch the buffering status. If it changes, we may have to resume because the playing stopped because of bad network-conditions.
            BufferingChanged += (sender, e) =>
            {
                // If the player is ready to play, it's paused and the status is still on PLAYING, go on!
                var isPlaying = Status == MediaPlayerStatus.Playing;
                if (CurrentItem.Status == AVPlayerItemStatus.ReadyToPlay && Rate == 0.0f && isPlaying)
                    Player.Play();
            };
        }

        private AVPlayer Player
        {
            get
            {
                if (_player == null)
                    InitializePlayer();
                
                return _player;
            }
        }

        private NSObject PeriodicTimeObserverObject;

        private AVPlayerItem CurrentItem => Player.CurrentItem;

        private NSUrl nsUrl { get; set; }

        public float Rate
        {
            get
            {
                if (Player != null)
                    return Player.Rate;
                return 0.0f;
            }
            set
            {
                if (Player != null)
                    Player.Rate = value;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (CurrentItem == null)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(CurrentItem.CurrentTime.Seconds);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (CurrentItem == null || CurrentItem.Duration.IsIndefinite ||
                    CurrentItem.Duration.IsInvalid)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(CurrentItem.Duration.Seconds);
            }
        }

        public TimeSpan Buffered
        {
            get
            {
                var buffered = TimeSpan.Zero;

                var currentItem = CurrentItem;

                var loadedTimeRanges = currentItem?.LoadedTimeRanges;

                if (currentItem != null && loadedTimeRanges.Any())
                {
                    var loadedSegments = loadedTimeRanges
                        .Select(timeRange =>
                        {
                            var timeRangeValue = timeRange.CMTimeRangeValue;

                            var startSeconds = timeRangeValue.Start.Seconds;
                            var durationSeconds = timeRangeValue.Duration.Seconds;

                            return startSeconds + durationSeconds;
                        });

                    var loadedSeconds = loadedSegments.Max();

                    buffered = TimeSpan.FromSeconds(loadedSeconds);
                }

                Console.WriteLine("Buffered size: " + buffered);

                return buffered;
            }
        }

        public async Task Stop()
        {
            if (CurrentItem == null)
                return;

            if (Player.Rate != 0.0)
                Player.Pause();

            CurrentItem.Seek(CMTime.FromSeconds(0d, 1));

            Status = MediaPlayerStatus.Stopped;
            await Task.FromResult(true);
        }

        public async Task Pause()
        {
            Status = MediaPlayerStatus.Paused;

            if (CurrentItem == null)
                return;

            if (Player.Rate != 0.0)
                Player.Pause();

            await Task.FromResult(true);
        }

        public MediaPlayerStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(_status));
            }
        }

        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;

        public async Task Seek(TimeSpan position)
        {
            await Task.Run(() => { CurrentItem?.Seek(CMTime.FromSeconds(position.TotalSeconds, 1)); });
        }

        private void InitializePlayer()
        {
            if (_player != null)
            {
                _player.RemoveTimeObserver(PeriodicTimeObserverObject);
                _player.RemoveObserver(this, (NSString)"rate", RateObservationContext.Handle);

                _player.Dispose();
            }

            _player = new AVPlayer();
            ((VolumeManagerImplementation)_volumeManager).player = _player;

            if (_versionHelper.SupportsAutomaticWaitPlayerProperty) {
                _player.AutomaticallyWaitsToMinimizeStalling = false;
            }

#if __IOS__ || __TVOS__
            var avSession = AVAudioSession.SharedInstance();
            // By setting the Audio Session category to AVAudioSessionCategorPlayback, audio will continue to play when the silent switch is enabled, or when the screen is locked.
            avSession.SetCategory(AVAudioSessionCategory.Playback);


            NSError activationError = null;
            avSession.SetActive(true, out activationError);
            if (activationError != null)
                Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
#endif
            Player.AddObserver(this, (NSString)"rate", NSKeyValueObservingOptions.New |
                                                          NSKeyValueObservingOptions.Initial,
                                   RateObservationContext.Handle);

            PeriodicTimeObserverObject = Player.AddPeriodicTimeObserver(new CMTime(1, 4), DispatchQueue.MainQueue, delegate
            {
                if (CurrentItem.Duration.IsInvalid || CurrentItem.Duration.IsIndefinite || double.IsNaN(CurrentItem.Duration.Seconds))
                {
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(0, Position, Duration));
                }
                else
                {
                    var totalDuration = TimeSpan.FromSeconds(CurrentItem.Duration.Seconds);
                    var totalProgress = Position.TotalMilliseconds /
                                        totalDuration.TotalMilliseconds;
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(
                        !double.IsInfinity(totalProgress) ? totalProgress : 0,
                        Position,
                        Duration
                    ));
                }
            });
        }

        public async Task Play(IMediaFile mediaFile = null)
        {
            var sameMediaFile = mediaFile == null || mediaFile.Equals(_currentMediaFile);

            if (Status == MediaPlayerStatus.Paused && sameMediaFile)
            {
                Status = MediaPlayerStatus.Playing;
                //We are simply paused so just start again
                Player.Play();
                return;
            }

            if (mediaFile != null)
            {
                nsUrl = MediaFileUrlHelper.GetUrlFor(mediaFile);
                _currentMediaFile = mediaFile;
            }

            try
            {
                InitializePlayer();

                Status = MediaPlayerStatus.Buffering;

                var playerItem = GetPlayerItem(nsUrl);

                CurrentItem?.RemoveObserver(this, new NSString("status"));

                Player.ReplaceCurrentItemWithPlayerItem(playerItem);
                CurrentItem.AddObserver(this, new NSString("loadedTimeRanges"),
                    NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New,
                    LoadedTimeRangesObservationContext.Handle);

                CurrentItem.SeekingWaitsForVideoCompositionRendering = true;
                CurrentItem.AddObserver(this, (NSString)"status", NSKeyValueObservingOptions.New |
                                                                          NSKeyValueObservingOptions.Initial,
                    StatusObservationContext.Handle);


                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification,
                                                               notification => MediaFinished?.Invoke(this, new MediaFinishedEventArgs(mediaFile)), CurrentItem);

                Player.Play();
            }
            catch (Exception ex)
            {
                OnMediaFailed();
                Status = MediaPlayerStatus.Stopped;

                //unable to start playback log error
                Console.WriteLine("Unable to start playback: " + ex);
            }

            await Task.CompletedTask;
        }

        private AVPlayerItem GetPlayerItem(NSUrl url) {

            AVAsset asset;

            if (RequestHeaders != null && RequestHeaders.Any())
            {
                var options = MediaFileUrlHelper.GetOptionsWithHeaders(RequestHeaders);

                asset = AVUrlAsset.Create(url, options);
            }
            else
            {
                asset = AVAsset.FromUrl(url);
            }

            var playerItem = AVPlayerItem.FromAsset(asset);

            return playerItem;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            Console.WriteLine("Observer triggered for {0}", keyPath);

            switch (keyPath)
            {
                case "status":
                    ObserveStatus();
                    return;

                case "loadedTimeRanges":
                    ObserveLoadedTimeRanges();
                    return;

                case "rate":
                    ObserveRate();
                    return;

                default:
                    Console.WriteLine("Observer triggered for {0} not resolved ...", keyPath);
                    return;
            }
        }

        private void ObserveStatus()
        {
            Console.WriteLine("Status Observed Method {0}", CurrentItem.Status);

            var isBuffering = Status == MediaPlayerStatus.Buffering;

            if (CurrentItem.Status == AVPlayerItemStatus.ReadyToPlay && isBuffering)
            {
                Status = MediaPlayerStatus.Playing;
                Player.Play();
            }
            else if (CurrentItem.Status == AVPlayerItemStatus.Failed)
            {
                OnMediaFailed();
                Status = MediaPlayerStatus.Stopped;
            }
        }

        private void ObserveRate()
        {
            var stoppedPlaying = Rate == 0.0;

            if (stoppedPlaying && Status == MediaPlayerStatus.Playing)
            {
                //Update the status becuase the system changed the rate.
                Status = MediaPlayerStatus.Paused;
            }
        }

        private void OnMediaFailed()
        {
            var error = CurrentItem.Error;

            MediaFailed?.Invoke(this, new MediaFailedEventArgs(error.LocalizedDescription, new NSErrorException(error)));
        }

        private void ObserveLoadedTimeRanges()
        {
            var loadedTimeRanges = CurrentItem.LoadedTimeRanges;

            var hasLoadedAnyTimeRanges = loadedTimeRanges != null && loadedTimeRanges.Length > 0;

            if (hasLoadedAnyTimeRanges)
            {
                var range = loadedTimeRanges[0].CMTimeRangeValue;
                var duration = double.IsNaN(range.Duration.Seconds) ? TimeSpan.Zero : TimeSpan.FromSeconds(range.Duration.Seconds);
                var totalDuration = CurrentItem.Duration;
                var bufferProgress = duration.TotalSeconds / totalDuration.Seconds;
                BufferingChanged?.Invoke(this, new BufferingChangedEventArgs(
                    !double.IsInfinity(bufferProgress) ? bufferProgress : 0,
                    duration
                ));
            }
            else
            {
                BufferingChanged?.Invoke(this, new BufferingChangedEventArgs(0, TimeSpan.Zero));
            }
        }
    }
}
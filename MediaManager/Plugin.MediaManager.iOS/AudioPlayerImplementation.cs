using System;
using System.Collections.Generic;
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

        private IMediaFile _currentMediaFile;

        public static readonly NSString StatusObservationContext =
            new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");

        public static NSString RateObservationContext =
            new NSString("AVCustomEditPlayerViewControllerRateObservationContext");

        private AVPlayer _player;
        private MediaPlayerStatus _status;

        public Dictionary<string, string> RequestHeaders { get; set; }

        public AudioPlayerImplementation(IVolumeManager volumeManager)
        {
            _volumeManager = volumeManager;
            _status = MediaPlayerStatus.Stopped;

            // Watch the buffering status. If it changes, we may have to resume because the playing stopped because of bad network-conditions.
            BufferingChanged += (sender, e) =>
            {
                // If the player is ready to play, it's paused and the status is still on PLAYING, go on!
                if ((Player.Status == AVPlayerStatus.ReadyToPlay) && (Rate == 0.0f) &&
                    (Status == MediaPlayerStatus.Playing))
                    Player.Play();
            };
            _volumeManager.Mute = Player.Muted;
            _volumeManager.CurrentVolume = Player.Volume;
            _volumeManager.MaxVolume = 1;
            _volumeManager.VolumeChanged += VolumeManagerOnVolumeChanged;
        }

        private void VolumeManagerOnVolumeChanged(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            Player.Volume = (float)volumeChangedEventArgs.Volume;
            Player.Muted = volumeChangedEventArgs.Mute;
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
                if (Player.CurrentItem == null)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(Player.CurrentItem.CurrentTime.Seconds);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (Player.CurrentItem == null || Player.CurrentItem.Duration.IsIndefinite ||
                    Player.CurrentItem.Duration.IsInvalid)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(Player.CurrentItem.Duration.Seconds);
            }
        }

        public TimeSpan Buffered
        {
            get
            {
                var buffered = TimeSpan.Zero;

                var currentItem = Player.CurrentItem;

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
            await Task.Run(() =>
            {
                if (Player.CurrentItem == null)
                    return;

                if (Player.Rate != 0.0)
                    Player.Pause();

                Player.CurrentItem.Seek(CMTime.FromSeconds(0d, 1));

                Status = MediaPlayerStatus.Stopped;
            });
        }

        public async Task Pause()
        {
            await Task.Run(() =>
            {
                Status = MediaPlayerStatus.Paused;

                if (Player.CurrentItem == null)
                    return;

                if (Player.Rate != 0.0)
                    Player.Pause();
            });
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
            await Task.Run(() => { Player.CurrentItem?.Seek(CMTime.FromSeconds(position.TotalSeconds, 1)); });
        }

        private void InitializePlayer()
        {
            _player = new AVPlayer();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
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

            Player.AddPeriodicTimeObserver(new CMTime(1, 4), DispatchQueue.MainQueue, delegate
            {
                if (Player.CurrentItem.Duration.IsInvalid || Player.CurrentItem.Duration.IsIndefinite || double.IsNaN(Player.CurrentItem.Duration.Seconds))
                {
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(0, Position, Duration));
                }
                else
                {
                    var totalDuration = TimeSpan.FromSeconds(Player.CurrentItem.Duration.Seconds);
                    var totalProgress = Position.TotalMilliseconds /
                                        totalDuration.TotalMilliseconds;
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(totalProgress, Position, Duration));
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
                // Start off with the status LOADING.
                Status = MediaPlayerStatus.Buffering;

                var playerItem = GetPlayerItem(nsUrl);

                Player.CurrentItem?.RemoveObserver(this, new NSString("status"));

                Player.ReplaceCurrentItemWithPlayerItem(playerItem);
                playerItem.AddObserver(this, new NSString("status"), NSKeyValueObservingOptions.New, Player.Handle);
                playerItem.AddObserver(this, new NSString("loadedTimeRanges"),
                    NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, Player.Handle);

                Player.CurrentItem.SeekingWaitsForVideoCompositionRendering = true;
                Player.CurrentItem.AddObserver(this, (NSString)"status", NSKeyValueObservingOptions.New |
                                                                          NSKeyValueObservingOptions.Initial,
                    StatusObservationContext.Handle);

                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification,
                                                               notification => MediaFinished?.Invoke(this, new MediaFinishedEventArgs(mediaFile)), Player.CurrentItem);

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
                var options = GetOptionsWithHeaders(RequestHeaders);

                asset = AVUrlAsset.Create(url, options);
            }
            else
            {
                asset = AVAsset.FromUrl(url);
            }

            var playerItem = AVPlayerItem.FromAsset(asset);

            return playerItem;
        }

        private AVUrlAssetOptions GetOptionsWithHeaders(IDictionary<string, string> headers)
        {
            var nativeHeaders = new NSMutableDictionary();

            foreach (var header in headers)
            {
                nativeHeaders.Add((NSString)header.Key, (NSString)header.Value);
            }

            var nativeHeadersKey = (NSString) "AVURLAssetHTTPHeaderFieldsKey";

            var options = new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
                nativeHeaders,
                nativeHeadersKey
            ));

            return options;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            Console.WriteLine("Observer triggered for {0}", keyPath);

            switch ((string)keyPath)
            {
                case "status":
                    ObserveStatus();
                    return;

                case "loadedTimeRanges":
                    ObserveLoadedTimeRanges();
                    return;

                default:
                    Console.WriteLine("Observer triggered for {0} not resolved ...", keyPath);
                    return;
            }
        }

        private void ObserveStatus()
        {
            Console.WriteLine("Status Observed Method {0}", Player.Status);
            if ((Player.Status == AVPlayerStatus.ReadyToPlay) && (Status == MediaPlayerStatus.Buffering))
            {
                Status = MediaPlayerStatus.Playing;
                Player.Play();
            }
            else if (Player.Status == AVPlayerStatus.Failed)
            {
                OnMediaFailed();
                Status = MediaPlayerStatus.Stopped;
            }
        }

        private void OnMediaFailed()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Description: {Player.Error.LocalizedDescription}");
            builder.AppendLine($"Reason: {Player.Error.LocalizedFailureReason}");
            builder.AppendLine($"Recovery Options: {Player.Error.LocalizedRecoveryOptions}");
            builder.AppendLine($"Recovery Suggestion: {Player.Error.LocalizedRecoverySuggestion}");
            MediaFailed?.Invoke(this, new MediaFailedEventArgs(builder.ToString(), new NSErrorException(Player.Error)));
        }

        private void ObserveLoadedTimeRanges()
        {
            var loadedTimeRanges = Player.CurrentItem.LoadedTimeRanges;
            if (loadedTimeRanges.Length > 0)
            {
                var range = loadedTimeRanges[0].CMTimeRangeValue;
                var duration = TimeSpan.FromSeconds(range.Duration.Seconds);
                var totalDuration = Player.CurrentItem.Duration;
                var bufferProgress = duration.TotalSeconds / totalDuration.Seconds;
                BufferingChanged?.Invoke(this, new BufferingChangedEventArgs(bufferProgress, duration));
            }
            else
            {
                BufferingChanged?.Invoke(this, new BufferingChangedEventArgs(0, TimeSpan.Zero));
            }
        }
    }
}
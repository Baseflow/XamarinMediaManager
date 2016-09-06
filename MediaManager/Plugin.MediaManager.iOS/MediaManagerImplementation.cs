using System;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using Plugin.MediaManager.Abstractions;
using UIKit;

namespace Plugin.MediaManager
{
  /// <summary>
  /// Implementation for MediaManager
  /// </summary>
  public class MediaManagerImplementation : NSObject, IMediaManager
  {
        public IMediaQueue Queue { get; set; } = new MediaQueue();

        private AVPlayer _player;
        private AVPlayer player
        {
            get
            {
                if (_player == null)
                    InitializePlayer();

                return _player;
            }
        }

        NSUrl nsUrl { get; set; }

        public MediaManagerImplementation()
        {
            Status = PlayerStatus.STOPPED;

            // Watch the buffering status. If it changes, we may have to resume because the playing stopped because of bad network-conditions.
            Buffering += (sender, e) =>
            {
              // If the player is ready to play, it's paused and the status is still on PLAYING, go on!
              if (player.Status == AVPlayerStatus.ReadyToPlay && Rate == 0.0f && Status == PlayerStatus.PLAYING)
                {
                    player.Play();
                }
            };
        }

        public static NSString StatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");
        public static NSString RateObservationContext = new NSString("AVCustomEditPlayerViewControllerRateObservationContext");

        private void InitializePlayer()
        {
            _player = new AVPlayer();

            AVAudioSession avSession = AVAudioSession.SharedInstance();

            // By setting the Audio Session category to AVAudioSessionCategorPlayback, audio will continue to play when the silent switch is enabled, or when the screen is locked.
            avSession.SetCategory(AVAudioSessionCategory.Playback);

            NSError activationError = null;
            avSession.SetActive(true, out activationError);
            if (activationError != null)
            {
                Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
            }

            player.AddPeriodicTimeObserver(new CMTime(1, 4), DispatchQueue.MainQueue, delegate (CMTime time)
            {
                OnPlaying(EventArgs.Empty);
            });
        }

        public event StatusChangedEventHandler StatusChanged;

        public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingEventHandler Playing;

        public event BufferingEventHandler Buffering;

        public event TrackFinishedEventHandler TrackFinished;

        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, e);
        }

        protected virtual void OnCoverReloaded(EventArgs e)
        {
            if (CoverReloaded != null)
                CoverReloaded(this, e);
        }

        protected virtual void OnPlaying(EventArgs e)
        {
            Console.WriteLine("Playing Position: " + Position);

            if (Playing != null)
                Playing(this, e);
        }

        protected virtual void OnBuffering(EventArgs e)
        {
            if (Buffering != null)
                Buffering(this, e);
        }

        public async Task Play(IMediaFile mediaFile)
        {
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                    await Play(mediaFile.Url);
                    break;
                case MediaFileType.VideoUrl:
                    throw new NotImplementedException();
                    break;
                case MediaFileType.AudioFile:
                    throw new NotImplementedException();
                    break;
                case MediaFileType.VideoFile:
                    throw new NotImplementedException();
                    break;
                case MediaFileType.Other:
                    throw new NotImplementedException();
                    break;
                default:
                    await Task.FromResult(0);
                    break;
            }
        }

        public async Task Play(string url)
        {
            if (!string.IsNullOrEmpty(url))
                nsUrl = NSUrl.FromString(url);
            await Play();
        }

        public async Task Play()
        {
            await Play(async () => await PlayNext());
        }

        public async Task Play(Func<Task> ifNotAvailable)
        {
            if (Status == PlayerStatus.PAUSED)
            {
                Status = PlayerStatus.PLAYING;
                //We are simply paused so just start again
                player.Play();
                return;
            }

            try
            {
                // Start off with the status LOADING.
                Status = PlayerStatus.BUFFERING;

                Cover = null;

                AVAsset nsAsset = AVUrlAsset.FromUrl(nsUrl);
                AVPlayerItem streamingItem = AVPlayerItem.FromAsset(nsAsset);

                nsAsset.LoadValuesAsynchronously(new string[] { "commonMetadata" }, delegate
                {
                    foreach (AVMetadataItem item in streamingItem.Asset.CommonMetadata)
                    {
                        if (item.KeySpace == AVMetadata.KeySpaceID3 && item.CommonKey == AVMetadata.CommonKeyArtwork)
                        {
                            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                                Cover = UIImage.LoadFromData(item.Value as NSData);
                            else
                            {
                                NSObject data;
                                (item.Value as NSMutableDictionary).TryGetValue(new NSString("data"), out data);
                                Cover = UIImage.LoadFromData(data as NSData);
                            }
                        }
                    }

                    if (Cover == null)
                    {
                        Cover = UIImage.FromFile("placeholder_cover.png");
                    }
                });

                if (player.CurrentItem != null)
                {
                    // Remove the observer before destructing the current item
                    player.CurrentItem.RemoveObserver(this, new NSString("status"));
                }

                player.ReplaceCurrentItemWithPlayerItem(streamingItem);
                streamingItem.AddObserver(this, new NSString("status"), NSKeyValueObservingOptions.New, player.Handle);
                streamingItem.AddObserver(this, new NSString("loadedTimeRanges"), NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, player.Handle);

                player.CurrentItem.SeekingWaitsForVideoCompositionRendering = true;
                player.CurrentItem.AddObserver(this, (NSString)"status", NSKeyValueObservingOptions.New |
                    NSKeyValueObservingOptions.Initial, StatusObservationContext.Handle);

                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, async (notification) =>
                {
                    await PlayNext();
                }, player.CurrentItem);

                player.Play();
            }
            catch (Exception ex)
            {
                Status = PlayerStatus.STOPPED;

                //unable to start playback log error
                Console.WriteLine("Unable to start playback: " + ex);
            }
        }

        private PlayerStatus status;

        public PlayerStatus Status
        {
            get
            {
                return status;
            }
            private set
            {
                status = value;
                OnStatusChanged(EventArgs.Empty);
            }
        }

        public async Task Stop()
        {
            await Task.Run(() =>
            {
                if (player.CurrentItem == null)
                    return;

                if (player.Rate != 0.0)
                    player.Pause();

                player.CurrentItem.Seek(CMTime.FromSeconds(0d, 1));

                Status = PlayerStatus.STOPPED;
            });
        }

        public async Task Pause()
        {
            await Task.Run(() =>
            {
                Status = PlayerStatus.PAUSED;

                if (player.CurrentItem == null)
                {
                    return;
                }

                if (player.Rate != 0.0)
                    player.Pause();
            });
        }

        public async Task Seek(int position)
        {
            await Task.Run(() =>
            {
                if (player.CurrentItem != null)
                    player.CurrentItem.Seek(CMTime.FromSeconds((double)position / 1000, 1));
            });
        }

        public async Task PlayNext()
        {
            if (Queue.HasNext())
            {
                await Stop();

                Queue.SetNextAsCurrent();
                await Play();
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                await Stop();
                Queue.SetIndexAsCurrent(0);
                Cover = null;
            }
        }

        public async Task PlayPrevious()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (!Queue.HasPrevious() || Position > 3000)
            {
                await Seek(0);
            }
            else
            {
                await Stop();

                Queue.SetPreviousAsCurrent();
                await Play();
            }
        }

        public async Task PlayPause()
        {
            if (Status == PlayerStatus.PAUSED || Status == PlayerStatus.STOPPED)
            {
                await Play();
            }
            else
            {
                await Pause();
            }
        }

        public float Rate
        {
            get
            {
                if (player != null)
                {
                    return player.Rate;
                }
                else
                {
                    return 0.0f;
                }
            }
            set
            {
                if (player != null)
                {
                    player.Rate = value;
                }
            }
        }

        public int Position
        {
            get
            {
                if (player.CurrentItem == null)
                    return -1;
                else
                    return (int)(player.CurrentItem.CurrentTime.Seconds * 1000);
            }
        }

        public int Duration
        {
            get
            {
                if (player.CurrentItem == null)
                    return 0;
                else
                    return (int)(player.CurrentItem.Duration.Seconds * 1000);
            }
        }

        public int Buffered
        {
            get
            {
                var buffered = 0;
                if (player.CurrentItem != null)
                {
                    buffered = (int)(player.CurrentItem.LoadedTimeRanges.Select(tr => tr.CMTimeRangeValue.Start.Seconds + tr.CMTimeRangeValue.Duration.Seconds).Max() * 1000);
                }

                Console.WriteLine("Buffered size: " + buffered);

                return buffered;
            }
        }



        private UIImage cover;
        public object Cover
        {
            get
            {
                if (player.CurrentItem == null)
                    return null;
                else
                    return cover;
            }
            private set
            {
                cover = value as UIImage;
                OnCoverReloaded(EventArgs.Empty);
            }
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
            Console.WriteLine("Status Observed Method {0}", player.Status);
            if (player.Status == AVPlayerStatus.ReadyToPlay && Status == PlayerStatus.BUFFERING)
            {
                Status = PlayerStatus.PLAYING;
                player.Play();
            }
            else if (player.Status == AVPlayerStatus.Failed)
            {
                Status = PlayerStatus.STOPPED;
                Console.WriteLine("Stream Failed");
            }
        }

        private void ObserveLoadedTimeRanges()
        {
            OnBuffering(EventArgs.Empty);
        }
    }
}
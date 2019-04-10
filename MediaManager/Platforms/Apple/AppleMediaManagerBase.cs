using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Apple;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public abstract class AppleMediaManagerBase<TMediaPlayer> : MediaManagerBase<TMediaPlayer, AVPlayer> where TMediaPlayer : AppleMediaPlayer, IMediaPlayer<AVPlayer>, new()
    {
        private IMediaPlayer _mediaPlayer;
        public override IMediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new TMediaPlayer();
                }
                return _mediaPlayer;
            }
            set
            {
                _mediaPlayer = value;
            }
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                {
                    _mediaExtractor = new AppleMediaExtractor();
                }
                return _mediaExtractor;
            }
            set
            {
                _mediaExtractor = value;
            }
        }

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager(NativeMediaPlayer.Player);
                return _volumeManager;
            }
            set
            {
                _volumeManager = value;
            }
        }

        public override MediaPlayerState State
        {
            get
            {
                return MediaPlayer.State;
            }
        }

        public override TimeSpan Position
        {
            get
            {
                if (NativeMediaPlayer.Player.CurrentItem == null)
                {
                    return TimeSpan.Zero;
                }
                return TimeSpan.FromSeconds(NativeMediaPlayer.Player.CurrentItem.CurrentTime.Seconds);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                if (NativeMediaPlayer.Player.CurrentItem == null)
                {
                    return TimeSpan.Zero;
                }
                if (double.IsNaN(NativeMediaPlayer.Player.CurrentItem.Duration.Seconds))
                {
                    return TimeSpan.Zero;
                }
                return TimeSpan.FromSeconds(NativeMediaPlayer.Player.CurrentItem.Duration.Seconds);
            }
        }

        public override TimeSpan Buffered
        {
            get
            {
                var buffered = TimeSpan.Zero;
                if (NativeMediaPlayer.Player.CurrentItem != null)
                {
                    buffered =
                        TimeSpan.FromSeconds(
                            NativeMediaPlayer.Player.CurrentItem.LoadedTimeRanges.Select(
                                tr => tr.CMTimeRangeValue.Start.Seconds + tr.CMTimeRangeValue.Duration.Seconds).Max());
                }

                return buffered;
            }
        }

        public override float Speed
        {
            get
            {
                if (NativeMediaPlayer.Player != null)
                    return NativeMediaPlayer.Player.Rate;
                return 0.0f;
            }
            set
            {
                if (NativeMediaPlayer.Player != null)
                    NativeMediaPlayer.Player.Rate = value;
            }
        }

        public override void Init()
        {
            MediaPlayer.Initialize();
            IsInitialized = true;
        }

        public override Task Pause()
        {
            this.MediaPlayer.Pause();
            return Task.CompletedTask;
        }

        public override Task Play(IMediaItem mediaItem)
        {
            base.Play(mediaItem);
            IsInitialized = true;

            MediaPlayer.Play(MediaQueue.Current);
            return Task.CompletedTask;
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await base.Play(uri);

            await MediaPlayer.Play(MediaQueue.Current);
            return mediaItem;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            await base.Play(items);

            await this.MediaPlayer.Play(MediaQueue.Current);
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            await base.Play(items);

            await this.MediaPlayer.Play(MediaQueue.Current);
            return MediaQueue;
        }

        public override async Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }

        public override Task Play()
        {
            this.MediaPlayer.Play();
            return Task.CompletedTask;
        }

        public override Task SeekTo(TimeSpan position)
        {
            return this.MediaPlayer.Seek(position);
        }

        public override Task StepBackward()
        {
            var playerTime = NativeMediaPlayer.Player.CurrentTime;
            return this.SeekTo(TimeSpan.FromSeconds(Double.IsNaN(playerTime.Seconds) ? 0 : ((playerTime.Seconds < 10) ? 0 : playerTime.Seconds - 10)));
        }

        public override Task StepForward()
        {
            var playerTime = NativeMediaPlayer.Player.CurrentTime;
            return this.SeekTo(TimeSpan.FromSeconds(Double.IsNaN(playerTime.Seconds) ? 0 : playerTime.Seconds + 10));
        }

        public override Task Stop()
        {
            return this.MediaPlayer.Stop();
        }

        public override RepeatMode RepeatMode
        {
            get
            {
                return MediaPlayer.RepeatMode;
            }
            set
            {
                MediaPlayer.RepeatMode = value;
            }
        }

        public override void ToggleShuffle()
        {
            this.MediaQueue.Shuffle();
        }
    }
}

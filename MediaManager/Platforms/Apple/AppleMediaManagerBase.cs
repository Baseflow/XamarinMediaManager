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
            IsInitialized = true;
            this.MediaPlayer.Play(mediaItem);
            return Task.CompletedTask;
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            MediaQueue.Clear();
            var mediaItem = await MediaExtractor.CreateMediaItem(uri);

            this.MediaQueue.Add(mediaItem);
            await this.MediaPlayer.Play();
            return mediaItem;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            MediaQueue.Clear();
            foreach (var item in items)
            {
                MediaQueue.Add(item);
            }
            await this.MediaPlayer.Play();
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            MediaQueue.Clear();

            foreach (var uri in items)
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(uri);
                MediaQueue.Add(mediaItem);
            }

            await this.MediaPlayer.Play();
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

        public override Task<bool> PlayNext()
        {
            // If we repeat just the single media item, we do that first
            if (MediaPlayer.RepeatMode == RepeatMode.One)
            {
                MediaPlayer.Play(MediaQueue.Current);
                return Task.FromResult(true);
            }
            else {
                // Otherwise we try to play the next media item in the queue
                if (MediaQueue.HasNext())
                {
                    MediaPlayer.Play(MediaQueue.NextItem);
                    return Task.FromResult(true);
                }
                else
                {
                    // If there is no next media item, but we repeat them all, we reset the current index and start playing it again
                    if (MediaPlayer.RepeatMode == RepeatMode.All)
                    {
                        // Go to the start of the queue again
                        MediaQueue.CurrentIndex = 0;
                        if (MediaQueue.HasCurrent())
                        {
                            MediaPlayer.Play(MediaQueue.Current);
                        }
                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }

        public override Task PlayPrevious()
        {
            if (MediaQueue.HasPrevious())
            {
                MediaPlayer.Play(MediaQueue.PreviousItem);
            }

            return Task.CompletedTask;
        }

        public override Task SeekTo(TimeSpan position)
        {
            return this.MediaPlayer.Seek(position);
        }

        public override Task StepBackward()
        {
            throw new NotImplementedException();
        }

        public override Task StepForward()
        {
            throw new NotImplementedException();
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

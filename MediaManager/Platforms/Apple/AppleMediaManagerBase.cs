using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public abstract class AppleMediaManagerBase<TMediaPlayer> : MediaManagerBase<TMediaPlayer, AVQueuePlayer> where TMediaPlayer : class, IMediaPlayer<AVQueuePlayer>, new()
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

        public override IMediaExtractor MediaExtractor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IVolumeManager VolumeManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override MediaPlayerState State => throw new NotImplementedException();

        public override TimeSpan Position
        {
            get
            {
                if (NativeMediaPlayer.Player.CurrentItem == null)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(NativeMediaPlayer.Player.CurrentItem.CurrentTime.Seconds);
            }
        }

        public override TimeSpan Duration
        {
            get
            {
                if (NativeMediaPlayer.Player.CurrentItem == null)
                    return TimeSpan.Zero;
                if (double.IsNaN(NativeMediaPlayer.Player.CurrentItem.Duration.Seconds))
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(NativeMediaPlayer.Player.CurrentItem.Duration.Seconds);
            }
        }

        public override TimeSpan Buffered
        {
            get
            {
                var buffered = TimeSpan.Zero;
                if (NativeMediaPlayer.Player.CurrentItem != null)
                    buffered =
                        TimeSpan.FromSeconds(
                            NativeMediaPlayer.Player.CurrentItem.LoadedTimeRanges.Select(
                                tr => tr.CMTimeRangeValue.Start.Seconds + tr.CMTimeRangeValue.Duration.Seconds).Max());

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
            throw new NotImplementedException();
        }

        public override Task Pause()
        {
            this.MediaPlayer.Pause();
            return Task.CompletedTask;
        }

        public override Task Play(IMediaItem mediaItem)
        {
            this.MediaPlayer.Play(mediaItem);
            return Task.CompletedTask;
        }

        public override Task<IMediaItem> Play(string uri)
        {
            throw new NotImplementedException();
        }

        public override Task Play(IEnumerable<IMediaItem> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IMediaItem> Play(FileInfo file)
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

        public override Task PlayNext()
        {
            throw new NotImplementedException();
        }

        public override Task PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public override Task SeekTo(TimeSpan position)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void ToggleRepeat()
        {
            throw new NotImplementedException();
        }

        public override void ToggleShuffle()
        {
            throw new NotImplementedException();
        }
    }
}

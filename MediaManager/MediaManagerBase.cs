using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public abstract class MediaManagerBase<TMediaPlayer, TPlayer> : MediaManagerBase, IMediaManager<TMediaPlayer, TPlayer> where TMediaPlayer : class, IMediaPlayer<TPlayer> where TPlayer : class
    {
        public TMediaPlayer NativeMediaPlayer => MediaPlayer as TMediaPlayer;
    }

    public abstract class MediaManagerBase : IMediaManager, INotifyMediaManager
    {
        public MediaManagerBase()
        {
            Timer.AutoReset = true;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        public TimeSpan StepSize { get; set; } = TimeSpan.FromSeconds(10);
        
        public bool IsInitialized { get; protected set; }

        public abstract IMediaPlayer MediaPlayer { get; set; }

        public abstract IMediaExtractor MediaExtractor { get; set; }
        public abstract IVolumeManager VolumeManager { get; set; }

        private IMediaQueue _mediaQueue;
        public virtual IMediaQueue MediaQueue
        {
            get
            {
                if (_mediaQueue == null)
                    _mediaQueue = new MediaQueue();

                return _mediaQueue;
            }
            set
            {
                _mediaQueue = value;
            }
        }

        public abstract MediaPlayerState State { get; }
        public abstract TimeSpan Position { get; }
        public abstract TimeSpan Duration { get; }
        public abstract TimeSpan Buffered { get; }
        public abstract float Speed { get; set; }
        public abstract RepeatMode RepeatMode { get; set; }
        public abstract ShuffleMode ShuffleMode { get; set; }

        public abstract Task Pause();
        public abstract Task Play(IMediaItem mediaItem);
        public abstract Task<IMediaItem> Play(string uri);
        public abstract Task Play(IEnumerable<IMediaItem> items);
        public abstract Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items);
        public virtual Task<IMediaItem> AddMediaItemsToQueue(IEnumerable<IMediaItem> items, bool clearQueue = false)
        {
            if (clearQueue)
            {
                MediaQueue.Clear();
            }

            foreach (var item in items)
            {
                MediaQueue.Add(item);
            }

            return Task.FromResult(MediaQueue.Current);
        }
        public abstract Task<IMediaItem> Play(FileInfo file);
        public abstract Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo);
        public abstract Task Play();
        public virtual Task<bool> PlayNext()
        {
            // If we repeat just the single media item, we do that first
            if (MediaPlayer.RepeatMode == RepeatMode.One)
            {
                MediaPlayer.Play(MediaQueue.Current);
                return Task.FromResult(true);
            }
            else
            {
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
                            return Task.FromResult(true);
                        }
                    }
                }
            }

            return Task.FromResult(false);
        }

        public virtual Task<bool> PlayPrevious()
        {
            if (MediaQueue.HasPrevious())
            {
                MediaPlayer.Play(MediaQueue.PreviousItem);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        public abstract Task SeekTo(TimeSpan position);


        public virtual Task StepBackward()
        {
            var seekTo = this.SeekTo(TimeSpan.FromSeconds(Double.IsNaN(Position.Seconds) ? 0 : ((Position.Seconds < StepSize.Seconds) ? 0 : Position.Seconds - StepSize.Seconds)));
            Timer_Elapsed(null, null);
            return seekTo;
        }

        public virtual Task StepForward()
        {
            var seekTo = this.SeekTo(TimeSpan.FromSeconds(Double.IsNaN(Position.Seconds) ? 0 : Position.Seconds + StepSize.Seconds));
            Timer_Elapsed(null, null);
            return seekTo;
        }
        public abstract Task Stop();

        public void ToggleRepeat()
        {
            if (RepeatMode == (int)RepeatMode.Off)
            {
                RepeatMode = RepeatMode.All;
            }
            else
            {
                RepeatMode = RepeatMode.Off;
            }
        }

        public void ToggleShuffle()
        {
            if (ShuffleMode == (int)ShuffleMode.Off)
            {
                ShuffleMode = ShuffleMode.All;
            }
            else
            {
                ShuffleMode = ShuffleMode.Off;
            }
        }

        public Timer Timer { get; } = new Timer(1000);
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event StateChangedEventHandler StateChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event PositionChangedEventHandler PositionChanged;

        public event MediaItemFinishedEventHandler MediaItemFinished;
        public event MediaItemChangedEventHandler MediaItemChanged;
        public event MediaItemFailedEventHandler MediaItemFailed;

        public void OnBufferingChanged(object sender, BufferingChangedEventArgs e) => BufferingChanged?.Invoke(sender, e);
        public void OnMediaItemChanged(object sender, MediaItemEventArgs e) => MediaItemChanged?.Invoke(sender, e);
        public void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e) => MediaItemFailed?.Invoke(sender, e);
        public void OnMediaItemFinished(object sender, MediaItemEventArgs e) => MediaItemFinished?.Invoke(sender, e);
        public void OnPlayingChanged(object sender, PlayingChangedEventArgs e) => PlayingChanged?.Invoke(sender, e);
        public void OnStateChanged(object sender, StateChangedEventArgs e) => StateChanged?.Invoke(sender, e);
        public void OnPositionChanged(object sender, PositionChangedEventArgs e) => PositionChanged?.Invoke(sender, e);

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TimeSpan PreviousPosition = new TimeSpan();
        protected virtual void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsInitialized)
                return;

            if (PreviousPosition != Position)
            {
                PreviousPosition = Position;
                OnPositionChanged(this, new PositionChangedEventArgs(Position));
            }
            if (this.IsPlaying())
            {
                OnPlayingChanged(this, new PlayingChangedEventArgs(Position, Duration));
            }
            if(this.IsBuffering())
            {
                OnBufferingChanged(this, new BufferingChangedEventArgs(Buffered));
            }
        }

        public abstract void Init();
    }
}

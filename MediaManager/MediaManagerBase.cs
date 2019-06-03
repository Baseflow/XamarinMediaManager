using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;

namespace MediaManager
{
    public abstract class MediaManagerBase : IMediaManager
    {
        public MediaManagerBase()
        {
            Timer.AutoReset = true;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        public bool IsInitialized { get; protected set; }

        public Timer Timer { get; } = new Timer(1000);

        private TimeSpan _stepSize = TimeSpan.FromSeconds(10);
        public TimeSpan StepSize
        {
            get => _stepSize;
            set => SetProperty(ref _stepSize, value);
        }

        private Dictionary<string, string> _requestHeaders = new Dictionary<string, string>();
        public Dictionary<string, string> RequestHeaders
        {
            get => _requestHeaders;
            set => SetProperty(ref _requestHeaders, value);
        }

        private IMediaQueue _mediaQueue;
        public virtual IMediaQueue MediaQueue
        {
            get
            {
                if (_mediaQueue == null)
                    _mediaQueue = new MediaQueue();

                return _mediaQueue;
            }
            set => SetProperty(ref _mediaQueue, value);
        }

        public abstract void Init();
        public abstract IMediaPlayer MediaPlayer { get; set; }
        public abstract IMediaExtractor MediaExtractor { get; set; }
        public abstract IVolumeManager VolumeManager { get; set; }
        public abstract INotificationManager NotificationManager { get; set; }

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
        public abstract Task<IMediaItem> Play(FileInfo file);
        public abstract Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo);
        public abstract Task Play();
        public abstract Task Stop();
        public abstract Task SeekTo(TimeSpan position);

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

        public virtual async Task<bool> PlayNext()
        {
            // If we repeat just the single media item, we do that first
            if (MediaPlayer.RepeatMode == RepeatMode.One)
            {
                await MediaPlayer.Play(MediaQueue.Current);
                return true;
            }
            else
            {
                // Otherwise we try to play the next media item in the queue
                if (MediaQueue.HasNext())
                {
                    await MediaPlayer.Play(MediaQueue.NextItem);
                    return true;
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
                            await MediaPlayer.Play(MediaQueue.Current);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public virtual async Task<bool> PlayPrevious()
        {
            if (MediaQueue.HasPrevious())
            {
                await MediaPlayer.Play(MediaQueue.PreviousItem);
                return true;
            }

            return false;
        }

        public virtual async Task<bool> PlayQueueItem(IMediaItem mediaItem)
        {
            if (!MediaQueue.Contains(mediaItem))
                return false;

            await MediaPlayer.Play(mediaItem);
            return true;
        }

        public virtual Task StepBackward()
        {
            var seekTo = this.SeekTo(TimeSpan.FromSeconds(Double.IsNaN(Position.TotalSeconds) ? 0 : ((Position.TotalSeconds < StepSize.TotalSeconds) ? 0 : Position.TotalSeconds - StepSize.TotalSeconds)));
            Timer_Elapsed(null, null);
            return seekTo;
        }

        public virtual Task StepForward()
        {
            var seekTo = this.SeekTo(TimeSpan.FromSeconds(Double.IsNaN(Position.TotalSeconds) ? 0 : Position.TotalSeconds + StepSize.TotalSeconds));
            Timer_Elapsed(null, null);
            return seekTo;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event StateChangedEventHandler StateChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event PositionChangedEventHandler PositionChanged;

        public event MediaItemFinishedEventHandler MediaItemFinished;
        public event MediaItemChangedEventHandler MediaItemChanged;
        public event MediaItemFailedEventHandler MediaItemFailed;

        internal void OnBufferingChanged(object sender, BufferingChangedEventArgs e) => BufferingChanged?.Invoke(sender, e);
        internal void OnMediaItemChanged(object sender, MediaItemEventArgs e) => MediaItemChanged?.Invoke(sender, e);
        internal void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e) => MediaItemFailed?.Invoke(sender, e);
        internal void OnMediaItemFinished(object sender, MediaItemEventArgs e) => MediaItemFinished?.Invoke(sender, e);
        internal void OnPlayingChanged(object sender, PlayingChangedEventArgs e) => PlayingChanged?.Invoke(sender, e);

        private MediaPlayerState internalState = MediaPlayerState.Stopped;
        internal void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State != internalState)
            {
                internalState = e.State;
                StateChanged?.Invoke(sender, e);
            }
        }

        internal void OnPositionChanged(object sender, PositionChangedEventArgs e) => PositionChanged?.Invoke(sender, e);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
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
            if (this.IsBuffering())
            {
                OnBufferingChanged(this, new BufferingChangedEventArgs(Buffered));
            }
        }
    }
}

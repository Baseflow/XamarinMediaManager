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
using System.Linq;
using MediaManager.Player;
using MediaManager.Notifications;

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

        public Timer Timer { get; protected set; } = new Timer(1000);

        protected TimeSpan _stepSize = TimeSpan.FromSeconds(10);
        public virtual TimeSpan StepSize
        {
            get => _stepSize;
            set => SetProperty(ref _stepSize, value);
        }

        protected Dictionary<string, string> _requestHeaders = new Dictionary<string, string>();
        public virtual Dictionary<string, string> RequestHeaders
        {
            get => _requestHeaders;
            set => SetProperty(ref _requestHeaders, value);
        }

        protected IMediaQueue _mediaQueue;
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

        public virtual void Init()
        {
            IsInitialized = true;
        }

        public abstract IMediaPlayer MediaPlayer { get; set; }
        public abstract IMediaExtractor MediaExtractor { get; set; }
        public abstract IVolumeManager VolumeManager { get; set; }
        public abstract INotificationManager NotificationManager { get; set; }

        protected MediaPlayerState _state = MediaPlayerState.Stopped;
        public MediaPlayerState State
        {
            get => _state;
            internal set
            {
                if (SetProperty(ref _state, value))
                    OnStateChanged(this, new StateChangedEventArgs(State));
            }
        }

        protected TimeSpan _buffered;
        public TimeSpan Buffered
        {
            get => _buffered;
            internal set
            {
                if (SetProperty(ref _buffered, value))
                    OnBufferedChanged(this, new BufferedChangedEventArgs(Buffered));
            }
        }

        public abstract TimeSpan Position { get; }
        public abstract TimeSpan Duration { get; }
        public abstract float Speed { get; set; }
        public abstract RepeatMode RepeatMode { get; set; }

        public virtual ShuffleMode ShuffleMode
        {
            get
            {
                return MediaQueue.ShuffleMode;
            }
            set
            {
                MediaQueue.ShuffleMode = value;
            }
        }

        public bool ClearQueueOnPlay { get; set; } = true;

        public virtual Task Play()
        {
            return MediaPlayer.Play();
        }

        public virtual Task Pause()
        {
            return MediaPlayer.Pause();
        }

        public virtual Task SeekTo(TimeSpan position)
        {
            return MediaPlayer.SeekTo(position);
        }

        public virtual Task Stop()
        {
            return MediaPlayer.Stop();
        }

        public virtual async Task<IMediaItem> Play(IMediaItem mediaItem)
        {
            var mediaItemToPlay = await PrepareQueueForPlayback(mediaItem);
            await PlayAsCurrent(mediaItemToPlay);
            return mediaItemToPlay;
        }

        public virtual async Task<IMediaItem> Play(string uri)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(uri).ConfigureAwait(false);
            var mediaItemToPlay = await PrepareQueueForPlayback(mediaItem);

            await PlayAsCurrent(mediaItemToPlay);
            return mediaItem;
        }

        public virtual async Task<IMediaItem> Play(IEnumerable<IMediaItem> items)
        {
            var mediaItemToPlay = await PrepareQueueForPlayback(items.ToArray());

            await PlayAsCurrent(mediaItemToPlay);
            return mediaItemToPlay;
        }

        public virtual async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var uri in items)
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(uri).ConfigureAwait(false);
                mediaItems.Add(mediaItem);
            }

            var mediaItemToPlay = await PrepareQueueForPlayback(mediaItems.ToArray());
            await PlayAsCurrent(mediaItemToPlay);
            return mediaItems;
        }

        public virtual async Task<IMediaItem> Play(FileInfo file)
        {
            var mediaItem = await MediaExtractor.CreateMediaItem(file).ConfigureAwait(false);
            var mediaItemToPlay = await PrepareQueueForPlayback(mediaItem);

            await PlayAsCurrent(mediaItemToPlay);
            return mediaItem;
        }

        public virtual async Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            var mediaItems = new List<IMediaItem>();
            foreach (var file in directoryInfo.GetFiles())
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(file).ConfigureAwait(false);
                mediaItems.Add(mediaItem);
            }
            var mediaItemToPlay = await PrepareQueueForPlayback(mediaItems.ToArray());
            await PlayAsCurrent(mediaItemToPlay);
            return mediaItems;
        }

        public virtual async Task PlayAsCurrent(IMediaItem mediaItem)
        {
            await MediaPlayer.Play(mediaItem);
        }

        public virtual Task<IMediaItem> PrepareQueueForPlayback(params IMediaItem[] items)
        {
            if (ClearQueueOnPlay)
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
            if (RepeatMode == RepeatMode.One)
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
                    if (RepeatMode == RepeatMode.All)
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

        public virtual async Task<bool> PlayQueueItem(int index)
        {
            var mediaItem = MediaQueue.ElementAtOrDefault(index);
            if(mediaItem == null)
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
        public event BufferedChangedEventHandler BufferedChanged;
        public event PositionChangedEventHandler PositionChanged;

        public event MediaItemFinishedEventHandler MediaItemFinished;
        public event MediaItemChangedEventHandler MediaItemChanged;
        public event MediaItemFailedEventHandler MediaItemFailed;

        internal void OnBufferedChanged(object sender, BufferedChangedEventArgs e) => BufferedChanged?.Invoke(sender, e);
        internal void OnMediaItemChanged(object sender, MediaItemEventArgs e) => MediaItemChanged?.Invoke(sender, e);
        internal void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e) => MediaItemFailed?.Invoke(sender, e);
        internal void OnMediaItemFinished(object sender, MediaItemEventArgs e) => MediaItemFinished?.Invoke(sender, e);
        internal void OnPositionChanged(object sender, PositionChangedEventArgs e) => PositionChanged?.Invoke(sender, e);

        internal void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            StateChanged?.Invoke(sender, e);

            //TODO: Find a better way to trigger some changes.
            OnPropertyChanged(nameof(Duration));

            NotificationManager?.UpdateNotification();
        }

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

        protected TimeSpan PreviousPosition = new TimeSpan();

        protected virtual void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsInitialized)
                return;

            if (PreviousPosition != Position)
            {
                PreviousPosition = Position;
                //OnPropertyChanged(nameof(Position));
                OnPositionChanged(this, new PositionChangedEventArgs(Position));
            }
        }

        public virtual void Dispose()
        {
            Timer.Elapsed -= Timer_Elapsed;
            Timer.Dispose();
        }
    }
}

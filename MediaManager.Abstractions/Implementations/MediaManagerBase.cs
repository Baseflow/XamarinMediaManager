using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public abstract class MediaManagerBase : IMediaManager, IDisposable
    {
        public abstract IAudioPlayer AudioPlayer { get; set; }

        public abstract IVideoPlayer VideoPlayer { get; set; }

        public abstract INotificationManager NotificationManager { get; set; }

        public abstract IMediaExtractor MediaExtractor { get; set; }

        public abstract IVolumeManager VolumeManager { get; set; }

        public virtual IMediaQueue MediaQueue { get; set; } = new MediaQueue();

        public virtual IPlaybackController PlaybackController { get; set; }

        public IPlaybackManager CurrentPlaybackManager { get; private set; }

        protected MediaManagerBase()
        {
            PlaybackController = new PlaybackController(this);
        }

        public Task Play(string url, MediaItemType type)
        {
            return Play(new MediaItem(url, type));
        }

        public Task Play(IMediaItem item)
        {
            switch (item.Type)
            {
                case MediaItemType.Audio:
                    CurrentPlaybackManager = AudioPlayer;
                    return AudioPlayer.Play(item);
                case MediaItemType.Video:
                    CurrentPlaybackManager = VideoPlayer;
                    return VideoPlayer.Play(item);
                default:
                    return Task.FromResult(0);
                    //return Task.CompletedTask;
            }
        }

        public Task Play(IEnumerable<IMediaItem> items)
        {
            MediaQueue.AddRange(items);
            return Play(MediaQueue.FirstOrDefault());
        }

        /*public Task Play(Stream stream, MediaItemType type)
        {
            switch (type)
            {
                case MediaItemType.Audio:
                    CurrentPlaybackManager = AudioPlayer;
                    return AudioPlayer.Play(stream);
                case MediaItemType.Video:
                    CurrentPlaybackManager = VideoPlayer;
                    return VideoPlayer.Play(stream);
                default:
                    return Task.CompletedTask;
            }
        }

        public Task Play(FileInfo file, MediaItemType type)
        {
            switch (type)
            {
                case MediaItemType.Audio:
                    CurrentPlaybackManager = AudioPlayer;
                    return AudioPlayer.Play(file);
                case MediaItemType.Video:
                    CurrentPlaybackManager = VideoPlayer;
                    return VideoPlayer.Play(file);
                default:
                    return Task.CompletedTask;
            }
        }*/

        /*
        private IPlaybackManager _currentPlaybackManager;

        private Func<IMediaItem, Task> _onBeforePlay;

        private IPlaybackManager CurrentPlaybackManager
        {
            get
            {
                if (_currentPlaybackManager == null && CurrentMediaItem != null) SetCurrentPlayer(CurrentMediaItem.Type);

                if (_currentPlaybackManager != null)
                {
                    _currentPlaybackManager.RequestHeaders = RequestHeaders;
                }

                return _currentPlaybackManager;
            }
        }

        public virtual IMediaQueue MediaQueue { get; set; } = new MediaQueue();

        public abstract IAudioPlayer AudioPlayer { get; set; }

        public abstract IVideoPlayer VideoPlayer { get; set; }

        public abstract INotificationManager MediaNotificationManager { get; set; }

        public abstract IMediaExtractor MediaExtractor { get; set; }

        public abstract IVolumeManager VolumeManager { get; set; }

        public IPlaybackController PlaybackController { get; set; }

        public PlaybackState Status => CurrentPlaybackManager?.Status ?? PlaybackState.Stopped;

        public TimeSpan Position => CurrentPlaybackManager?.Position ?? TimeSpan.Zero;

        public TimeSpan Duration => CurrentPlaybackManager?.Duration ?? TimeSpan.Zero;

        public TimeSpan Buffered => CurrentPlaybackManager?.Buffered ?? TimeSpan.Zero;

        public event StatusChangedEventHandler StatusChanged;

        public event PlayingChangedEventHandler PlayingChanged;

        public event BufferingChangedEventHandler BufferingChanged;

        public event MediaFinishedEventHandler MediaFinished;

        public event MediaFailedEventHandler MediaFailed;

        public event MediaItemChangedEventHandler MediaItemChanged;

        public event MediaItemFailedEventHandler MediaItemFailed;

        private IMediaItem CurrentMediaItem => MediaQueue.Current;

        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        private bool _startedPlaying;

        protected MediaManagerBase()
        {
            PlaybackController = new PlaybackController(this);
        }

        public async Task PlayNext()
        {
            await RaiseMediaItemFailedEventOnException(async () =>
            {
                if (MediaQueue.HasNext())
                {
                    MediaQueue.SetNextAsCurrent();

                    await PlayCurrent();
                }
                else
                {
                    MediaQueue.SetIndexAsCurrent(0);

                    await PrepareCurrentAndThen(async () =>
                    {
                        await CurrentPlaybackManager.Play();        
                        await CurrentPlaybackManager.Pause();
                        await Seek(TimeSpan.Zero);
                    });

                    OnMediaItemChanged(this, new MediaItemChangedEventArgs(CurrentMediaItem));
                }
            });
        }

        public async Task PlayPrevious()
        {
            await RaiseMediaItemFailedEventOnException(async () =>
            {
                MediaQueue.SetPreviousAsCurrent();

                await PlayCurrent();
            });
        }

        public async Task PlayByPosition(int index)
        {
            MediaQueue.SetIndexAsCurrent(index);
            await Play(CurrentMediaItem);
        }

        public Task Play(string url)
        {
            var MediaItem = new MediaItem(url);
            return Play(MediaItem);
        }

        public Task Play(string url, MediaItemType fileType)
        {
            var MediaItem = new MediaItem(url, fileType);
            return Play(MediaItem);
        }

        public Task Play(string url, MediaItemType fileType, ResourceAvailability availability)
        {
            var MediaItem = new MediaItem(url, fileType, availability);
            return Play(MediaItem);
        }

        public async Task Play(IMediaItem MediaItem = null)
        {
            if (MediaItem == null)
            {
                if (Status == PlaybackState.Paused)
                {
                    await Resume();
                    return;
                }

                MediaItem = CurrentMediaItem;
            }

            if (_currentPlaybackManager != null && Status == PlaybackState.Failed)
            {
                await PlayNext();
                return;
            }

            if (MediaItem == null)
            {
                await Play(MediaQueue);
                return;
            }

            if (!MediaQueue.Contains(MediaItem))
                MediaQueue.Add(MediaItem);

            MediaQueue.SetTrackAsCurrent(MediaItem);            

            await RaiseMediaItemFailedEventOnException(async () =>
            {
                await PlayCurrent();
            });

            MediaNotificationManager?.StartNotification(MediaItem);
        }

        /// <summary>
        /// Adds all MediaItems to the Queue and starts playing the first item
        /// </summary>
        /// <param name="MediaItems"></param>
        /// <returns></returns>
        public async Task Play(IEnumerable<IMediaItem> MediaItems)
        {
            MediaQueue.Clear();
            MediaQueue.AddRange(MediaItems);

            await PlayNext();

            MediaNotificationManager?.StartNotification(CurrentMediaItem);
        }

        public void SetOnBeforePlay(Func<IMediaItem, Task> beforePlay)
        {
            _onBeforePlay = beforePlay;
        }

        public async Task Pause()
        {
            IPlaybackManager currentPlaybackManager = CurrentPlaybackManager;
            if (currentPlaybackManager != null)
                await currentPlaybackManager.Pause();
        }

        public async Task Stop()
        {
            if (CurrentPlaybackManager == null)
                return;
            await CurrentPlaybackManager.Stop();
            MediaNotificationManager?.StopNotifications();
        }

        public async Task Seek(TimeSpan position)
        {
            if (CurrentPlaybackManager == null)
                return;
            await CurrentPlaybackManager.Seek(position);
            MediaNotificationManager?.UpdateNotifications(CurrentMediaItem, Status);
        }

        private async Task Resume()
        {
            if (CurrentPlaybackManager == null)
                return;
            await CurrentPlaybackManager.Play(CurrentMediaItem);
        }

        private async Task RaiseMediaItemFailedEventOnException(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                OnMediaItemFailed(CurrentPlaybackManager, new MediaItemFailedEventArgs(ex, CurrentMediaItem));
            }
        }

        private Task PlayCurrent()
        {
            return PrepareCurrentAndThen(() => CurrentPlaybackManager.Play(CurrentMediaItem));
        }

        private async Task PrepareCurrentAndThen(Func<Task> action = null)
        {
            await ExecuteOnBeforePlay();

            if(CurrentPlaybackManager != null)
                await Task.WhenAll(
                    action?.Invoke(),
                    ExtractMediaInformation(CurrentMediaItem));
        }

        private async Task ExecuteOnBeforePlay()
        {
            var beforePlayTask = _onBeforePlay?.Invoke(CurrentMediaItem);
            if (beforePlayTask != null) await beforePlayTask;
        }

        private void SetCurrentPlayer(MediaItemType fileType)
        {
            if (_currentPlaybackManager != null)
            {
                RemoveEventHandlers();
            }
            switch (fileType)
            {
                case MediaItemType.Audio:
                    _currentPlaybackManager = AudioPlayer;
                    break;
                case MediaItemType.Video:
                    _currentPlaybackManager = VideoPlayer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            AddEventHandlers();
        }

        private async Task ExtractMediaInformation(IMediaItem MediaItem)
        {
            var index = MediaQueue.IndexOf(MediaItem);
            await MediaExtractor.ExtractMediaInfo(MediaItem);

            if (index >= 0)
            {
                MediaQueue[index] = MediaItem;
            }

            OnMediaItemChanged(CurrentPlaybackManager, new MediaItemChangedEventArgs(MediaItem));
        }

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender != CurrentPlaybackManager) return;

            if (Status == PlaybackState.Playing)
            {
                _startedPlaying = false;
            }

            MediaNotificationManager?.UpdateNotifications(CurrentMediaItem, e.Status);
            StatusChanged?.Invoke(sender, e);
        }

        private void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
            {
                if (!_startedPlaying && Duration != TimeSpan.Zero)
                {
                    MediaNotificationManager?.UpdateNotifications(MediaQueue.Current, Status);
                    _startedPlaying = true;
                }

                PlayingChanged?.Invoke(sender, e);
            }
        }

        private async void OnMediaFinished(object sender, MediaFinishedEventArgs e)
        {
            if (sender != CurrentPlaybackManager) return;

            MediaFinished?.Invoke(sender, e);

            if (MediaQueue.Repeat == RepeatMode.RepeatOne)
            {
                await Seek(TimeSpan.Zero);
                await Resume();
            }
            else
            {
                await PlayNext();
            }
        }

        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
            {
                OnStatusChanged(sender, new StatusChangedEventArgs(PlaybackState.Failed));
            }
            MediaFailed?.Invoke(sender, e);
        }

        private void OnBufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                BufferingChanged?.Invoke(sender, e);
        }

        private void OnMediaItemChanged(object sender, MediaItemChangedEventArgs e)
        {
            if (CurrentMediaItem?.Url == e?.File?.Url)
                MediaNotificationManager?.UpdateNotifications(e?.File, Status);
            MediaItemChanged?.Invoke(sender, e);

        }

        private void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
            {
                OnStatusChanged(sender, new StatusChangedEventArgs(PlaybackState.Failed));
                MediaItemFailed?.Invoke(sender, e);
            }
        }

        private void AddEventHandlers()
        {
            _currentPlaybackManager.BufferingChanged += OnBufferingChanged;
            _currentPlaybackManager.MediaFailed += OnMediaFailed;
            _currentPlaybackManager.MediaFinished += OnMediaFinished;
            _currentPlaybackManager.PlayingChanged += OnPlayingChanged;
            _currentPlaybackManager.StatusChanged += OnStatusChanged;
        }

        private void RemoveEventHandlers()
        {
            _currentPlaybackManager.BufferingChanged -= OnBufferingChanged;
            _currentPlaybackManager.MediaFailed -= OnMediaFailed;
            _currentPlaybackManager.MediaFinished -= OnMediaFinished;
            _currentPlaybackManager.PlayingChanged -= OnPlayingChanged;
            _currentPlaybackManager.StatusChanged -= OnStatusChanged;
        }*/

        #region IDisposable        
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //RemoveEventHandlers();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~MediaManagerBase()
        {
            Dispose(false);
        }
        #endregion
    }
}

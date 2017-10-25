using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        //public IPlaybackManager CurrentPlaybackManager { get; private set; }

        protected MediaManagerBase()
        {
            PlaybackController = new PlaybackController(this);
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


        private IPlaybackManager _currentPlaybackManager;

        private Func<IMediaItem, Task> _onBeforePlay;

        public IPlaybackManager CurrentPlaybackManager
        {
            [DebuggerStepThrough]
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

        public PlaybackState State => CurrentPlaybackManager?.State ?? PlaybackState.Stopped;

        public TimeSpan Position => CurrentPlaybackManager?.Position ?? TimeSpan.Zero;

        public TimeSpan Duration => CurrentPlaybackManager?.Duration ?? TimeSpan.Zero;

        public TimeSpan Buffered => CurrentPlaybackManager?.Buffered ?? TimeSpan.Zero;

        public event StatusChangedEventHandler Status;

        public event PlayingChangedEventHandler Playing;

        public event BufferingChangedEventHandler Buffering;

        public event MediaFinishedEventHandler Finished;

        public event MediaFailedEventHandler Failed;

        public event MediaItemChangedEventHandler MediaItemChanged;

        public event MediaItemFailedEventHandler MediaItemFailed;

        private IMediaItem CurrentMediaItem
        {
            [DebuggerStepThrough]
            get { return MediaQueue.Current; }
        }

        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        private bool _startedPlaying;

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
                        await CurrentPlaybackManager.Play(CurrentMediaItem);
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

        public Task Play()
        {
            return PlaybackController.Play();
        }

        public Task Play(string url, MediaItemType fileType)
        {
            var MediaItem = new MediaItem(url, fileType);
            return Play(MediaItem);
        }

        public async Task Play(IMediaItem MediaItem = null)
        {
            if (MediaItem == null)
            {
                if (State == PlaybackState.Paused)
                {
                    await Resume();
                    return;
                }

                MediaItem = CurrentMediaItem;
            }

            if (_currentPlaybackManager != null && State == PlaybackState.Failed)
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

            //await RaiseMediaItemFailedEventOnException(async () =>
            //{
            //    await PlayCurrent();
            //});
            await AudioPlayer.Play(MediaItem);

            NotificationManager?.StartNotification(MediaItem);
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

            NotificationManager?.StartNotification(CurrentMediaItem);
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
            NotificationManager?.StopNotifications();
        }

        public async Task Seek(TimeSpan position)
        {
            if (CurrentPlaybackManager == null)
                return;
            await CurrentPlaybackManager.Seek(position);
            NotificationManager?.UpdateNotifications(CurrentMediaItem, State);
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

            if (CurrentPlaybackManager != null)
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

            if (State == PlaybackState.Playing)
            {
                _startedPlaying = false;
            }

            NotificationManager?.UpdateNotifications(CurrentMediaItem, e.State);
            Status?.Invoke(sender, e);
        }

        private void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
            {
                if (!_startedPlaying && Duration != TimeSpan.Zero)
                {
                    NotificationManager?.UpdateNotifications(MediaQueue.Current, State);
                    _startedPlaying = true;
                }

                Playing?.Invoke(sender, e);
            }
        }

        private async void OnMediaFinished(object sender, MediaFinishedEventArgs e)
        {
            if (sender != CurrentPlaybackManager) return;

            Finished?.Invoke(sender, e);

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
            Failed?.Invoke(sender, e);
        }

        private void OnBufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                Buffering?.Invoke(sender, e);
        }

        private void OnMediaItemChanged(object sender, MediaItemChangedEventArgs e)
        {
            if (CurrentMediaItem?.Url == e?.Item?.Url)
                NotificationManager?.UpdateNotifications(e?.Item, State);
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
            _currentPlaybackManager.Buffering += OnBufferingChanged;
            _currentPlaybackManager.Failed += OnMediaFailed;
            _currentPlaybackManager.Finished += OnMediaFinished;
            _currentPlaybackManager.Playing += OnPlayingChanged;
            _currentPlaybackManager.Status += OnStatusChanged;
        }

        private void RemoveEventHandlers()
        {
            _currentPlaybackManager.Buffering -= OnBufferingChanged;
            _currentPlaybackManager.Failed -= OnMediaFailed;
            _currentPlaybackManager.Finished -= OnMediaFinished;
            _currentPlaybackManager.Playing -= OnPlayingChanged;
            _currentPlaybackManager.Status -= OnStatusChanged;
        }

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

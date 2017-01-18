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

        private IPlaybackManager _currentPlaybackManager;

        private Func<IMediaFile, Task> _onBeforePlay;

        private IPlaybackManager CurrentPlaybackManager
        {
            get
            {
                if (_currentPlaybackManager == null && CurrentMediaFile != null) SetCurrentPlayer(CurrentMediaFile.Type);

                if (_currentPlaybackManager != null)
                {
                    _currentPlaybackManager.RequestHeaders = RequestHeaders;
                }

                return _currentPlaybackManager;
            }
            set
            {
                _currentPlaybackManager = value;
            }
        }

        public virtual IMediaQueue MediaQueue { get; set; } = new MediaQueue();

        public abstract IAudioPlayer AudioPlayer { get; set; }

        public abstract IVideoPlayer VideoPlayer { get; set; }

        public abstract IMediaNotificationManager MediaNotificationManager { get; set; }

        public abstract IMediaExtractor MediaExtractor { get; set; }

        public abstract IVolumeManager VolumeManager { get; set; }

        public MediaPlayerStatus Status => CurrentPlaybackManager.Status;

        public TimeSpan Position => CurrentPlaybackManager.Position;

        public TimeSpan Duration => CurrentPlaybackManager.Duration;

        public TimeSpan Buffered => CurrentPlaybackManager.Buffered;

        public event StatusChangedEventHandler StatusChanged;

        public event PlayingChangedEventHandler PlayingChanged;

        public event BufferingChangedEventHandler BufferingChanged;

        public event MediaFinishedEventHandler MediaFinished;

        public event MediaFailedEventHandler MediaFailed;

        public event MediaFileChangedEventHandler MediaFileChanged;

        public event MediaFileFailedEventHandler MediaFileFailed;

        private IMediaFile CurrentMediaFile => MediaQueue.Current;

        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        public async Task PlayNext()
        {
            await EmitMediaFileFailedEventOnException(async () =>
            {
                if (MediaQueue.HasNext())
                {
                    MediaQueue.SetNextAsCurrent();

                    await PlayCurrent();
                }
                else
                {
                    await CurrentPlaybackManager.Pause();
                    MediaQueue.SetIndexAsCurrent(0);
                    OnMediaFileChanged(this, new MediaFileChangedEventArgs(CurrentMediaFile));
                }
            });
        }

        public async Task PlayPrevious()
        {
            await EmitMediaFileFailedEventOnException(async () =>
            {
                if (Position > TimeSpan.FromSeconds(3) || !MediaQueue.HasPrevious())
                {
                    await Seek(TimeSpan.Zero);
                }
                else
                {
                    MediaQueue.SetPreviousAsCurrent();

                    await PlayCurrent();
                }
            });
        }

        public async Task PlayByPosition(int index)
        {
            MediaQueue.SetIndexAsCurrent(index);
            await Play(CurrentMediaFile);
        }

        /// <summary>
        /// Adds MediaFile to the Queue and starts playing
        /// </summary>
        /// <param name="mediaFile"></param>
        /// <returns></returns>
        public async Task Play(IMediaFile mediaFile)
        {
            if (_currentPlaybackManager != null && Status == MediaPlayerStatus.Failed)
            {
                await PlayNext();
                return;
            }

            if (mediaFile == null)
            {
                await Play(MediaQueue);
                return;
            }

            if (!MediaQueue.Contains(mediaFile))
                MediaQueue.Add(mediaFile);

            MediaQueue.SetTrackAsCurrent(mediaFile);

            await EmitMediaFileFailedEventOnException(async () =>
            {
                await PlayCurrent();
            });

            MediaNotificationManager?.StartNotification(mediaFile);
        }

        /// <summary>
        /// Adds all MediaFiles to the Queue and starts playing the first item
        /// </summary>
        /// <param name="mediaFiles"></param>
        /// <returns></returns>
        public async Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            var enumerable = mediaFiles as IList<IMediaFile> ?? mediaFiles.ToList();
            MediaQueue.Clear();
            MediaQueue.AddRange(enumerable);

            await Task.WhenAll(
                PlayNext(),
                GetMediaInformation(enumerable));

            MediaNotificationManager?.StartNotification(CurrentMediaFile);
        }

        public async Task Play(string url, MediaFileType fileType)
        {
            await Play(new MediaFile(url, fileType));
        }

        public async Task PlayPause()
        {
            switch (Status)
            {
                case MediaPlayerStatus.Paused:
                    await CurrentPlaybackManager.Play(CurrentMediaFile);
                    break;
                case MediaPlayerStatus.Stopped:
                    await Play(CurrentMediaFile);
                    break;
                default:
                    await CurrentPlaybackManager.Pause();
                    break;
            }
        }

        public void SetOnBeforePlay(Func<IMediaFile, Task> beforePlay)
        {
            _onBeforePlay = beforePlay;
        }

        public async Task Pause()
        {
            await CurrentPlaybackManager.Pause();
        }

        public async Task Stop()
        {
            await CurrentPlaybackManager.Stop();
            MediaNotificationManager?.StopNotifications();
        }

        public async Task Seek(TimeSpan position)
        {
            await CurrentPlaybackManager.Seek(position);
        }

        private async Task EmitMediaFileFailedEventOnException(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                OnMediaFileFailed(CurrentPlaybackManager, new MediaFileFailedEventArgs(ex, CurrentMediaFile));
                throw;
            }
        }

        private async Task PlayCurrent()
        {
            await ExecuteOnBeforePlay();

            await Task.WhenAll(
                CurrentPlaybackManager.Play(CurrentMediaFile),
                GetMediaInformation(new[] { CurrentMediaFile }));
        }

        private async Task ExecuteOnBeforePlay()
        {
            var beforePlayTask = _onBeforePlay?.Invoke(CurrentMediaFile);
            if (beforePlayTask != null) await beforePlayTask;
        }

        private void SetCurrentPlayer(MediaFileType fileType)
        {
            if (_currentPlaybackManager != null)
            {
                RemoveEventHandlers();
            }
            switch (fileType)
            {
                case MediaFileType.AudioUrl:
                case MediaFileType.AudioFile:
                    _currentPlaybackManager = AudioPlayer;
                    break;
                case MediaFileType.VideoUrl:
                case MediaFileType.VideoFile:
                    _currentPlaybackManager = VideoPlayer;
                    break;
                case MediaFileType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            AddEventHandlers();
        }

        private async Task GetMediaInformation(IEnumerable<IMediaFile> mediaFiles)
        {
            foreach (var mediaFile in mediaFiles)
            {
                try
                {
                    var index = MediaQueue.IndexOf(mediaFile);
                    await MediaExtractor.ExtractMediaInfo(mediaFile);

                    if (index >= 0)
                    {
                        MediaQueue[index] = mediaFile;
                    }

                    OnMediaFileChanged(CurrentPlaybackManager, new MediaFileChangedEventArgs(mediaFile));
                }
                catch (Exception e)
                {
                    OnMediaFileFailed(this, new MediaFileFailedEventArgs(e, mediaFile));
                }
            }
        }

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender != CurrentPlaybackManager) return;
            MediaNotificationManager?.UpdateNotifications(CurrentMediaFile, e.Status);
            StatusChanged?.Invoke(sender, e);
        }

        private void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                PlayingChanged?.Invoke(sender, e);
        }

        private async void OnMediaFinished(object sender, MediaFinishedEventArgs e)
        {
            if (sender != CurrentPlaybackManager) return;
            MediaFinished?.Invoke(sender, e);
            if (MediaQueue.Repeat == RepeatType.RepeatOne) await Seek(TimeSpan.Zero);
            else await PlayNext();
        }

        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
            {
                OnStatusChanged(sender, new StatusChangedEventArgs(MediaPlayerStatus.Failed));
            }
            MediaFailed?.Invoke(sender, e);
        }

        private void OnBufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                BufferingChanged?.Invoke(sender, e);
        }

        private void OnMediaFileChanged(object sender, MediaFileChangedEventArgs e)
        {
            if (CurrentMediaFile?.Url == e?.File?.Url)
                MediaNotificationManager?.UpdateNotifications(e?.File, Status);
            MediaFileChanged?.Invoke(sender, e);

        }

        private void OnMediaFileFailed(object sender, MediaFileFailedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
            {
                OnStatusChanged(sender, new StatusChangedEventArgs(MediaPlayerStatus.Failed));
                MediaFileFailed?.Invoke(sender, e);
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
        }

        public void Dispose()
        {
            RemoveEventHandlers();
        }
    }
}

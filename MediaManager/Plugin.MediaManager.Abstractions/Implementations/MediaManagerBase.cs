using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public abstract class MediaManagerBase : IMediaManager, IPlaybackManager, IDisposable
    {
        private IPlaybackManager _currentPlaybackManager;
        private IMediaFile _currentMediaFile;

        public virtual IMediaQueue MediaQueue { get; set; } = new MediaQueue();
        public abstract IAudioPlayer AudioPlayer { get; set; }
        public abstract IVideoPlayer VideoPlayer { get; set; }
        public abstract IMediaNotificationManager MediaNotificationManager { get; set; }
        public abstract IMediaExtractor MediaExtractor { get; set; }

        /// <summary>
        /// Hooks up eventhandlers 
        /// </summary>
        protected void Init()
        {

        }

        public async Task PlayNext()
        {
            if (MediaQueue.HasNext())
            {
                await _currentPlaybackManager.Stop();
                var item = MediaQueue[MediaQueue.Index + 1];
                SetCurrentPlayer(item);
                MediaQueue.SetNextAsCurrent();
                await _currentPlaybackManager.Play(item);
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                await _currentPlaybackManager.Stop();
                var item = MediaQueue[0];
                SetCurrentPlayer(item);
                MediaQueue.SetIndexAsCurrent(0);
                //Cover = null;
            }
        }

        private void SetCurrentPlayer(IMediaFile item)
        {
            if (item != null)
            {
                RemoveEventHandlers();
                switch (item.Type)
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
        }

        public async Task PlayPrevious()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (!MediaQueue.HasPrevious() || (Position > TimeSpan.FromSeconds(3)))
            {
                await _currentPlaybackManager.Seek(TimeSpan.Zero);
            }
            else
            {
                await _currentPlaybackManager.Stop();
                var previousItem = MediaQueue[MediaQueue.Index - 1];
                SetCurrentPlayer(previousItem);
                MediaQueue.SetPreviousAsCurrent();
                await _currentPlaybackManager.Play(previousItem);
            }
        }

        public async Task PlayByPosition(int index)
        {
            var item = MediaQueue[index];
            SetCurrentPlayer(item);
            await _currentPlaybackManager.Play(item);
        }

        public MediaPlayerStatus Status => _currentPlaybackManager.Status;
        public TimeSpan Position => _currentPlaybackManager.Position;
        public TimeSpan Duration => _currentPlaybackManager.Duration;
        public TimeSpan Buffered => _currentPlaybackManager.Buffered;
        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFileChangedEventHandler MediaFileChanged;
        public event MediaFileFailedEventHandler MediaFileFailed;

        public async Task Play(IMediaFile mediaFile)
        {
            MediaQueue.Clear();
            MediaQueue.Add(mediaFile);

            _currentMediaFile = mediaFile;
            SetCurrentPlayer(mediaFile);
            await _currentPlaybackManager.Play(mediaFile);
        }

        public async Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            MediaQueue.Clear();
            MediaQueue.AddRange(mediaFiles);
            await PlayNext();
        }

        public async Task Play(string url, MediaFileType fileType)
        {
            await _currentPlaybackManager.Play(url, fileType);
        }

        public async Task PlayPause()
        {
            if ((Status == MediaPlayerStatus.Paused) || (Status == MediaPlayerStatus.Stopped))
                await _currentPlaybackManager.Play(_currentMediaFile);
            else
                await _currentPlaybackManager.Pause();
        }

        public async Task Pause()
        {
            await _currentPlaybackManager.Pause();
        }

        public async Task Stop()
        {
            await _currentPlaybackManager.Stop();
        }

        public async Task Seek(TimeSpan position)
        {
            await _currentPlaybackManager.Seek(position);
        }

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                StatusChanged?.Invoke(sender, e);
        }

        private void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                PlayingChanged?.Invoke(sender, e);
        }

        private void OnMediaFinished(object sender, MediaFinishedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                MediaFinished?.Invoke(sender, e);
        }

        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                MediaFailed?.Invoke(sender, e);
        }

        private void OnBufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                BufferingChanged?.Invoke(sender, e);
        }

        private void OnMediaFileChanged(object sender, MediaFileChangedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                MediaFileChanged?.Invoke(sender, e);
        }

        private void OnMediaFileFailed(object sender, MediaFileFailedEventArgs e)
        {
            if (sender == _currentPlaybackManager)
                MediaFileFailed?.Invoke(sender, e);
        }

        private void AddEventHandlers()
        {
            _currentPlaybackManager.BufferingChanged += OnBufferingChanged;
            _currentPlaybackManager.MediaFailed += OnMediaFailed;
            _currentPlaybackManager.MediaFinished += OnMediaFinished;
            _currentPlaybackManager.PlayingChanged += OnPlayingChanged;
            _currentPlaybackManager.StatusChanged += OnStatusChanged;
            _currentPlaybackManager.MediaFileChanged += OnMediaFileChanged;
            _currentPlaybackManager.MediaFileFailed += OnMediaFileFailed;
        }

        private void RemoveEventHandlers()
        {
            _currentPlaybackManager.BufferingChanged -= OnBufferingChanged;
            _currentPlaybackManager.MediaFailed -= OnMediaFailed;
            _currentPlaybackManager.MediaFinished -= OnMediaFinished;
            _currentPlaybackManager.PlayingChanged -= OnPlayingChanged;
            _currentPlaybackManager.StatusChanged -= OnStatusChanged;
            _currentPlaybackManager.MediaFileChanged -= OnMediaFileChanged;
            _currentPlaybackManager.MediaFileFailed -= OnMediaFileFailed;
        }

        public void Dispose()
        {
            RemoveEventHandlers();
        }
    }
}
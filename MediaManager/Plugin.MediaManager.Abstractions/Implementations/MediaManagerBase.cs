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
        private IPlaybackManager CurrentPlaybackManager { 
            get {
                if (_currentPlaybackManager == null && _currentMediaFile != null)
                    SetCurrentPlayer(_currentMediaFile);
                else if (_currentPlaybackManager == null)
                    throw new Exception("No player is set");
                return _currentPlaybackManager;
            }
            set
            {
                _currentPlaybackManager = value;
            }
        }
        private IMediaFile _currentMediaFile { get; set; }

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
                await CurrentPlaybackManager.Stop();
                var item = MediaQueue[MediaQueue.Index + 1];
                //SetCurrentPlayer(item);
                MediaQueue.SetNextAsCurrent();
                await CurrentPlaybackManager.Play(item);
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                await CurrentPlaybackManager.Stop();
                var item = MediaQueue[0];
                //SetCurrentPlayer(item);
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
                        CurrentPlaybackManager = AudioPlayer;
                        break;
                    case MediaFileType.VideoUrl:
                    case MediaFileType.VideoFile:
                        CurrentPlaybackManager = VideoPlayer;
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
                await CurrentPlaybackManager.Seek(TimeSpan.Zero);
            }
            else
            {
                await CurrentPlaybackManager.Stop();
                var previousItem = MediaQueue[MediaQueue.Index - 1];
                //SetCurrentPlayer(previousItem);
                MediaQueue.SetPreviousAsCurrent();
                await CurrentPlaybackManager.Play(previousItem);
            }
        }

        public async Task PlayByPosition(int index)
        {
            var item = MediaQueue[index];
            //SetCurrentPlayer(item);
            await CurrentPlaybackManager.Play(item);
        }

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

        public async Task Play(IMediaFile mediaFile)
        {
            MediaQueue.Clear();
            MediaQueue.Add(mediaFile);

            _currentMediaFile = mediaFile;
            //SetCurrentPlayer(mediaFile);
            await CurrentPlaybackManager.Play(mediaFile);
        }

        public async Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            MediaQueue.Clear();
            MediaQueue.AddRange(mediaFiles);
            await PlayNext();
        }

        public async Task Play(string url, MediaFileType fileType)
        {
            await Play(new MediaFile(url, fileType));
        }

        public async Task PlayPause()
        {
            if ((Status == MediaPlayerStatus.Paused) || (Status == MediaPlayerStatus.Stopped))
                await CurrentPlaybackManager.Play(_currentMediaFile);
            else
                await CurrentPlaybackManager.Pause();
        }

        public async Task Pause()
        {
            await CurrentPlaybackManager.Pause();
        }

        public async Task Stop()
        {
            await CurrentPlaybackManager.Stop();
        }

        public async Task Seek(TimeSpan position)
        {
            await CurrentPlaybackManager.Seek(position);
        }

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                StatusChanged?.Invoke(sender, e);
        }

        private void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                PlayingChanged?.Invoke(sender, e);
        }

        private void OnMediaFinished(object sender, MediaFinishedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                MediaFinished?.Invoke(sender, e);
        }

        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                MediaFailed?.Invoke(sender, e);
        }

        private void OnBufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                BufferingChanged?.Invoke(sender, e);
        }

        private void OnMediaFileChanged(object sender, MediaFileChangedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                MediaFileChanged?.Invoke(sender, e);
        }

        private void OnMediaFileFailed(object sender, MediaFileFailedEventArgs e)
        {
            if (sender == CurrentPlaybackManager)
                MediaFileFailed?.Invoke(sender, e);
        }

        private void AddEventHandlers()
        {
            CurrentPlaybackManager.BufferingChanged += OnBufferingChanged;
            CurrentPlaybackManager.MediaFailed += OnMediaFailed;
            CurrentPlaybackManager.MediaFinished += OnMediaFinished;
            CurrentPlaybackManager.PlayingChanged += OnPlayingChanged;
            CurrentPlaybackManager.StatusChanged += OnStatusChanged;
            CurrentPlaybackManager.MediaFileChanged += OnMediaFileChanged;
            CurrentPlaybackManager.MediaFileFailed += OnMediaFileFailed;
        }

        private void RemoveEventHandlers()
        {
            if (_currentPlaybackManager != null)
            {
                CurrentPlaybackManager.BufferingChanged -= OnBufferingChanged;
                CurrentPlaybackManager.MediaFailed -= OnMediaFailed;
                CurrentPlaybackManager.MediaFinished -= OnMediaFinished;
                CurrentPlaybackManager.PlayingChanged -= OnPlayingChanged;
                CurrentPlaybackManager.StatusChanged -= OnStatusChanged;
                CurrentPlaybackManager.MediaFileChanged -= OnMediaFileChanged;
                CurrentPlaybackManager.MediaFileFailed -= OnMediaFileFailed;
            }
        }

        public void Dispose()
        {
            RemoveEventHandlers();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.Abstractions.Implementations
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public abstract class MediaManagerBase : IMediaManager, IDisposable
    {
        private IPlaybackManager _currentPlaybackManager;
        private IPlaybackManager CurrentPlaybackManager { 
            get {
                if (_currentPlaybackManager == null && _currentMediaFile != null)
                    SetCurrentPlayer(_currentMediaFile.Type);
                else if (_currentPlaybackManager == null)
                    throw new Exception("No player is set");
                return _currentPlaybackManager;
            }
            set
            {
                _currentPlaybackManager = value;
            }
        }
        private IMediaFile _currentMediaFile => MediaQueue.Current;

        public void Dispose()
        {
            RemoveEventHandlers();
        }

        public virtual IMediaQueue MediaQueue { get; protected set; } =  new MediaQueue();
        public abstract IAudioPlayer AudioPlayer { get; set; }
        public abstract IVideoPlayer VideoPlayer { get; set; }
        public abstract IMediaNotificationManager MediaNotificationManager { get; set; }
        public abstract IMediaExtractor MediaExtractor { get; set; }

        public async Task PlayNext()
        {
            if (MediaQueue.HasNext())
            {
                await CurrentPlaybackManager.Stop();
                MediaQueue.SetNextAsCurrent();
                await Task.WhenAll(
                    CurrentPlaybackManager.Play(_currentMediaFile),
                    GetMediaInformation(new[] { _currentMediaFile }));
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                await CurrentPlaybackManager.Stop();
                //var item = MediaQueue[0];
                MediaQueue.SetIndexAsCurrent(0);
                //Cover = null;
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
                MediaQueue.SetPreviousAsCurrent();
                await Task.WhenAll(
                   CurrentPlaybackManager.Play(_currentMediaFile),
                   GetMediaInformation(new[] { _currentMediaFile }));
            }
        }

        public async Task PlayByPosition(int index)
        {
            var item = MediaQueue[index];
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
            MediaQueue.Add(mediaFile);
            MediaQueue.SetTrackAsCurrent(mediaFile);
            
            await Task.WhenAll(
                CurrentPlaybackManager.Play(mediaFile), 
                GetMediaInformation(new[] {mediaFile}));
            MediaNotificationManager.StartNotification(mediaFile);
        }

        public async Task Play(IEnumerable<IMediaFile> mediaFiles)
        {
            var enumerable = mediaFiles as IList<IMediaFile> ?? mediaFiles.ToList();
            MediaQueue.Clear();
            MediaQueue.AddRange(enumerable);

            await Task.WhenAll(
                PlayNext(),
                GetMediaInformation(enumerable),
                Task.Run(() => MediaNotificationManager.StartNotification(MediaQueue.Current)));
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
                    await CurrentPlaybackManager.Play(_currentMediaFile);
                    break;
                case MediaPlayerStatus.Stopped:
                    await Play(MediaQueue.Current);
                    break;
                default:
                    await CurrentPlaybackManager.Pause();
                    break;
            }
        }

        public async Task Pause()
        {
            await CurrentPlaybackManager.Pause();
        }

        public async Task Stop()
        {
            await CurrentPlaybackManager.Stop();
            MediaNotificationManager.StopNotifications();
        }

        public async Task Seek(TimeSpan position)
        {
            await CurrentPlaybackManager.Seek(position);
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
                    var info = await MediaExtractor.ExtractMediaInfo(mediaFile);
                    MediaFileChanged?.Invoke(CurrentPlaybackManager, new MediaFileChangedEventArgs(info));
                }
                catch (Exception e)
                {
                    MediaFileFailed?.Invoke(this, new MediaFileFailedEventArgs(e, mediaFile));
                }
            }
        }

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender != CurrentPlaybackManager) return;
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
            if (MediaFinished != null)
                MediaFinished.Invoke(sender, e);
            else
                await PlayNext();
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
            if(sender == CurrentPlaybackManager)
                MediaNotificationManager.UpdateNotifications(e.File, Status);
        }

        private void AddEventHandlers()
        {
            _currentPlaybackManager.BufferingChanged += OnBufferingChanged;
            _currentPlaybackManager.MediaFailed += OnMediaFailed;
            _currentPlaybackManager.MediaFinished += OnMediaFinished;
            _currentPlaybackManager.PlayingChanged += OnPlayingChanged;
            _currentPlaybackManager.StatusChanged += OnStatusChanged;
            MediaFileChanged += OnMediaFileChanged;
        }

        private void RemoveEventHandlers()
        {
            _currentPlaybackManager.BufferingChanged -= OnBufferingChanged;
            _currentPlaybackManager.MediaFailed -= OnMediaFailed;
            _currentPlaybackManager.MediaFinished -= OnMediaFinished;
            _currentPlaybackManager.PlayingChanged -= OnPlayingChanged;
            _currentPlaybackManager.StatusChanged -= OnStatusChanged;
            MediaFileChanged -= OnMediaFileChanged;
        }
    }
}
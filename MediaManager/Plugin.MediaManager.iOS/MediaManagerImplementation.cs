using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for MediaManager
    /// </summary>
    public class MediaManagerImplementation : IMediaManager, IPlaybackManager, IDisposable
    {
        private IPlaybackManager _currentPlaybackManager;
        private IMediaFile _currentMediaFile;

        public MediaManagerImplementation()
        {
            _currentPlaybackManager = AudioPlayer;
            AudioPlayer.BufferingChanged += OnBufferingChanged;
            AudioPlayer.MediaFailed += OnMediaFailed;
            AudioPlayer.MediaFinished += OnMediaFinished;
            AudioPlayer.PlayingChanged += OnPlayingChanged;
            AudioPlayer.StatusChanged += OnStatusChanged;
        }

        public IMediaQueue Queue { get; } = new MediaQueue();
        public IAudioPlayer AudioPlayer { get; } = new AudioPlayerImplementation();
        public IVideoPlayer VideoPlayer { get; private set; }
        public IMediaQueue MediaQueue { get; private set; }
        public IMediaNotificationManager MediaNotificationManager { get; }
        public IMediaExtractor MediaExtractor { get; }

        public async Task PlayNext()
        {
            if (Queue.HasNext())
            {
                await _currentPlaybackManager.Stop();
                var item = Queue[Queue.Index + 1];
                SetCurrentPlayer(item);
                Queue.SetNextAsCurrent();
                await _currentPlaybackManager.Play(item.Url);
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                await _currentPlaybackManager.Stop();
                var item = Queue[0];
                SetCurrentPlayer(item);
                Queue.SetIndexAsCurrent(0);
                //Cover = null;
            }
        }

        private void SetCurrentPlayer(IMediaFile item)
        {
            if (item != null)
                switch (item.Type)
                {
                    case MediaFileType.AudioUrl:
                    case MediaFileType.AudioFile:
                        _currentPlaybackManager = AudioPlayer;
                        break;
                    case MediaFileType.VideoUrl:
                    case MediaFileType.VideoFile:
                        break;
                    case MediaFileType.Other:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        public async Task PlayPrevious()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (!Queue.HasPrevious() || (Position > TimeSpan.FromSeconds(3)))
            {
                await _currentPlaybackManager.Seek(TimeSpan.Zero);
            }
            else
            {
                await _currentPlaybackManager.Stop();
                var previousItem = Queue[Queue.Index - 1];
                SetCurrentPlayer(previousItem);
                Queue.SetPreviousAsCurrent();
                await _currentPlaybackManager.Play(previousItem);
            }
        }

        public async Task PlayByPosition(int index)
        {
            var item = Queue[index];
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


        public async Task Play(IMediaFile mediaFile)
        {
            _currentMediaFile = mediaFile;
            SetCurrentPlayer(mediaFile);
            await _currentPlaybackManager.Play(mediaFile);
        }

        public async Task Play(string url)
        {
            await _currentPlaybackManager.Play(url);
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

        public void Dispose()
        {
            AudioPlayer.BufferingChanged -= OnBufferingChanged;
            AudioPlayer.MediaFailed -= OnMediaFailed;
            AudioPlayer.MediaFinished -= OnMediaFinished;
            AudioPlayer.PlayingChanged -= OnPlayingChanged;
            AudioPlayer.StatusChanged -= OnStatusChanged;
        }
    }
}
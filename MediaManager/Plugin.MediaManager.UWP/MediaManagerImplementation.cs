using Plugin.MediaManager.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Plugin.MediaManager
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class MediaManagerImplementation : IMediaManager
    {
        private readonly MediaPlayer _player;
        private object _cover;
        private TaskCompletionSource<bool> _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
        private PlayerStatus _status;
        private readonly Timer _playProgressTimer;

        public MediaManagerImplementation()
        {
            _player = BackgroundMediaPlayer.Current;
            _playProgressTimer = new Timer(state =>
            {
                if (_player?.CurrentState == MediaPlayerState.Playing)
                {
                    Playing?.Invoke(this, EventArgs.Empty);
                }
            }, null, 0, int.MaxValue);

            _player.CurrentStateChanged += (sender, args) =>
            {
                switch (sender.CurrentState)
                {
                    case MediaPlayerState.Closed:
                        Status = PlayerStatus.STOPPED;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlayerState.Opening:
                        Status = PlayerStatus.BUFFERING;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlayerState.Buffering:
                        Status = PlayerStatus.BUFFERING;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlayerState.Playing:
                        Status = PlayerStatus.PLAYING;
                        _playProgressTimer.Change(0, 50);
                        break;
                    case MediaPlayerState.Paused:
                        Status = PlayerStatus.PAUSED;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlayerState.Stopped:
                        Status = PlayerStatus.STOPPED;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            _player.MediaEnded += (sender, args) => { TrackFinished?.Invoke(this, EventArgs.Empty); };
            _player.BufferingStarted += (sender, args) => { Buffering?.Invoke(this, EventArgs.Empty); };
            _player.BufferingEnded += (sender, args) => { Buffering?.Invoke(this, EventArgs.Empty); };

            _player.MediaFailed +=
                (sender, args) =>
                {
                    _playProgressTimer.Change(0, int.MaxValue);
                    _loadMediaTaskCompletionSource.SetException(new Exception("Media failed to load"));
                };

            _player.MediaOpened += (sender, args) =>
            {
                _loadMediaTaskCompletionSource.SetResult(true);
            };
        }

        public int Buffered
        {
            get
            {
                if (_player == null) return 0;
                return (int)(_player.BufferingProgress * _player.NaturalDuration.TotalMilliseconds);
            }
        }

        public object Cover
        {
            get { return _cover; }
            private set
            {
                _cover = value;
                CoverReloaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Duration => (int)(_player?.NaturalDuration.TotalMilliseconds ?? -1);
        public int Position => (int)(_player?.Position.TotalMilliseconds ?? 0);

        public IMediaQueue Queue { get; set; } = new MediaQueue();

        public PlayerStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event BufferingEventHandler Buffering;
        public event CoverReloadedEventHandler CoverReloaded;
        public event PlayingEventHandler Playing;
        public event StatusChangedEventHandler StatusChanged;
        public event TrackFinishedEventHandler TrackFinished;

        public Task Pause()
        {
            if (_player.CurrentState == MediaPlayerState.Paused)
            {
                _player.Play();
            }
            else
            {
                _player.Pause();
            }
            return Task.CompletedTask;
        }

        public async Task Play(IMediaFile mediaFile)
        {
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                    await Play(mediaFile.Url);
                    break;
                case MediaFileType.VideoUrl:
                    break;
                case MediaFileType.AudioFile:
                    break;
                case MediaFileType.VideoFile:
                    break;
                case MediaFileType.Other:
                    break;
                default:
                    await Task.FromResult(0);
                    break;
            }
        }

        public async Task Play(string url)
        {
            _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                var mediaSource = MediaSource.CreateFromUri(new Uri(url));
                _player.Source = mediaSource;
                await _loadMediaTaskCompletionSource.Task;
                await TryToLoadCover(url);
                _player.Play();
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to open url: " + url);
            }
        }

        private async Task TryToLoadCover(string url)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(url);
                using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300))
                {
                    if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                    {
                        Cover = thumbnail;
                    }
                    else
                    {
                        Debug.WriteLine("Unable to get cover for url: " + url);
                    }
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to get cover for url: " + url);
            }
        }

        public Task PlayNext()
        {
            if (Queue.HasNext())
            {
                Stop();

                Queue.SetNextAsCurrent();
                Play();
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                Stop();
                Queue.SetIndexAsCurrent(0);
                Cover = null;
            }
            return Task.CompletedTask;
        }

        public async Task PlayPause()
        {
            if (Status == PlayerStatus.PAUSED || Status == PlayerStatus.STOPPED)
            {
                await Play();
            }
            else
            {
                await Pause();
            }
        }

        public Task PlayPrevious()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (!Queue.HasPrevious() || Position > 3000)
            {
                Seek(0);
            }
            else
            {
                Stop();

                Queue.SetPreviousAsCurrent();
                Play();
            }
            return Task.CompletedTask;
        }

        public Task Seek(int position)
        {
            _player.Position = TimeSpan.FromSeconds(position);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            _player.PlaybackRate = 0;
            return Task.CompletedTask;
        }

        private Task Play()
        {
            _player.PlaybackRate = 1;
            _player.Play();
            return Task.CompletedTask;
        }
    }
}
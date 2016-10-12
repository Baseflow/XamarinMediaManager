using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    /// <summary>
    ///     Implementation for Feature
    /// </summary>
    public class MediaManagerImplementation : IMediaManager
    {
        private readonly MediaPlayer _player;
        private object _cover;
        private TaskCompletionSource<bool> _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> _seekTaskCompletionSource = new TaskCompletionSource<bool>();
        private PlayerStatus _status;
        private readonly Timer _playProgressTimer;

        public MediaManagerImplementation()
        {
            _player = new MediaPlayer();

            _playProgressTimer = new Timer(state =>
            {
                if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    Playing?.Invoke(this, EventArgs.Empty);
                }
            }, null, 0, int.MaxValue);



            _player.PlaybackSession.PlaybackStateChanged += (sender, args) =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.None:
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Opening:
                        Status = PlayerStatus.BUFFERING;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Buffering:
                        Status = PlayerStatus.BUFFERING;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Playing:
                        Status = PlayerStatus.PLAYING;
                        _playProgressTimer.Change(0, 50);
                        break;
                    case MediaPlaybackState.Paused:
                        Status = PlayerStatus.PAUSED;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            _player.MediaEnded += (sender, args) => { TrackFinished?.Invoke(this, EventArgs.Empty); };
            _player.BufferingStarted += (sender, args) => { Buffering?.Invoke(this, EventArgs.Empty); };
            _player.BufferingEnded += (sender, args) => { Buffering?.Invoke(this, EventArgs.Empty); };
            _player.PlaybackSession.SeekCompleted += (sender, args) => { };

            _player.MediaFailed += (sender, args) =>
            {
                _playProgressTimer.Change(0, int.MaxValue);
                _loadMediaTaskCompletionSource.SetException(new Exception("Media failed to load"));
            };

            _player.MediaOpened += (sender, args) => { _loadMediaTaskCompletionSource.SetResult(true); };
        }

        public int Buffered
        {
            get
            {
                if (_player == null) return 0;
                return (int)(_player.PlaybackSession.BufferingProgress * _player.PlaybackSession.NaturalDuration.TotalMilliseconds);
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

        public int Duration => (int)(_player?.PlaybackSession.NaturalDuration.TotalMilliseconds ?? -1);
        public int Position => (int)(_player?.PlaybackSession.Position.TotalMilliseconds ?? 0);

        public IMediaQueue Queue { get; set; }

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
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
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
            await Play(mediaFile.Url);
        }

        public async Task Play(string url)
        {
            _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                // Todo: sync this with the playback queue
                MediaPlaybackList mediaPlaybackList = new MediaPlaybackList();

                var mediaSource = MediaSource.CreateFromUri(new Uri(url));
                MediaPlaybackItem item = new MediaPlaybackItem(mediaSource);
                TryToLoadCover(item);
                mediaPlaybackList.Items.Add(item);
                _player.Source = mediaPlaybackList;
                await _loadMediaTaskCompletionSource.Task;
                _player.Play();
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to open url: " + url);
            }
        }

        private void TryToLoadCover(MediaPlaybackItem item)
        {
            try
            {
                Cover = item.GetDisplayProperties().Thumbnail;
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to get cover for item");
            }
        }

        public async Task PlayNext(string url)
        {
            await Stop();
            await Play(url);
        }

        public async Task PlayNext()
        {
            if (Queue.HasNext())
            {
                await Stop();

                Queue.SetNextAsCurrent();
                await Play();
            }
            else
            {
                // If you don't have a next song in the queue, stop and show the meta-data of the first song.
                await Stop();
                Queue.SetIndexAsCurrent(0);
                Cover = null;
            }
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

        public async Task PlayPrevious()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (!Queue.HasPrevious() || Position > 3000)
            {
                await Seek(0);
            }
            else
            {
                await Stop();

                Queue.SetPreviousAsCurrent();
                await Play();
            }
        }

        public async Task Seek(int position)
        {
            _seekTaskCompletionSource = new TaskCompletionSource<bool>();
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(position);
            await Task.CompletedTask;
        }

        public Task Stop()
        {
            _player.PlaybackSession.PlaybackRate = 0;
            _player.PlaybackSession.Position = TimeSpan.Zero;
            return Task.CompletedTask;
        }

        private Task Play()
        {
            _player.PlaybackSession.PlaybackRate = 1;
            _player.Play();
            return Task.CompletedTask;
        }
    }
}
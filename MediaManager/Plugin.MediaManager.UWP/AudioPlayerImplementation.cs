using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager
{
    public class AudioPlayerImplementation : IAudioPlayer
    {
        private readonly MediaPlayer _player;
        private readonly Timer _playProgressTimer;
        private TaskCompletionSource<bool> _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();

        public AudioPlayerImplementation()
        {
            _player = new MediaPlayer();

            _playProgressTimer = new Timer(state =>
            {
                if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    var progress = _player.PlaybackSession.Position.TotalSeconds/
                                   _player.PlaybackSession.NaturalDuration.TotalSeconds;
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(progress, _player.PlaybackSession.Position));
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
                        State = MediaPlayerStatus.Loading;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Buffering:
                        State = MediaPlayerStatus.Buffering;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Playing:
                        if (sender.PlaybackRate <= 0)
                        {
                            State = MediaPlayerStatus.Stopped;
                        }
                        else
                        {
                            State = MediaPlayerStatus.Playing;
                            _playProgressTimer.Change(0, 50);
                        }
                        break;
                    case MediaPlaybackState.Paused:
                        State = MediaPlayerStatus.Paused;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            _player.MediaEnded += (sender, args) => { MediaFinished?.Invoke(this, new MediaFinishedEventArgs()); };
            _player.PlaybackSession.BufferingProgressChanged += (sender, args) =>
            {
                var bufferedTime =
                    TimeSpan.FromSeconds(_player.PlaybackSession.BufferingProgress*
                                         _player.PlaybackSession.NaturalDuration.TotalSeconds);
                BufferingChanged?.Invoke(this,
                    new BufferingChangedEventArgs(_player.PlaybackSession.BufferingProgress, bufferedTime));
            };

            _player.PlaybackSession.SeekCompleted += (sender, args) => { };

            _player.MediaFailed += (sender, args) =>
            {
                _playProgressTimer.Change(0, int.MaxValue);
                _loadMediaTaskCompletionSource.SetException(new Exception("Media failed to load"));
            };

            _player.MediaOpened += (sender, args) => { _loadMediaTaskCompletionSource.SetResult(true); };
        }

        public IMediaQueue Queue { get; set; }

        public MediaPlayerStatus State
        {
            get { return Status; }
            private set
            {
                Status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(Status));
            }
        }

        public event StatusChangedEventHandler StatusChanged;
        public event PlayingChangedEventHandler PlayingChanged;
        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFinishedEventHandler MediaFinished;
        public event MediaFailedEventHandler MediaFailed;

        public TimeSpan Buffered
        {
            get
            {
                if (_player == null) return TimeSpan.Zero;
                return
                    TimeSpan.FromMilliseconds(_player.PlaybackSession.BufferingProgress*
                                              _player.PlaybackSession.NaturalDuration.TotalMilliseconds);
            }
        }

        public TimeSpan Duration => _player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;
        public MediaPlayerStatus Status { get; private set; }

        public TimeSpan Position => _player?.PlaybackSession.Position ?? TimeSpan.Zero;

        public Task Pause()
        {
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
                _player.Play();
            else
                _player.Pause();
            return Task.CompletedTask;
        }

        public async Task PlayPause()
        {
            if ((Status == MediaPlayerStatus.Paused) || (Status == MediaPlayerStatus.Stopped))
                await Play();
            else
                await Pause();
        }

        public async Task Play(IMediaFile mediaFile)
        {
            await Play(mediaFile.Url);
        }

        public async Task Play(string url, MediaFileType fileType)
        {
            await Play(url);
        }

        public async Task Seek(TimeSpan position)
        {
            _player.PlaybackSession.Position = position;
            await Task.CompletedTask;
        }

        public Task Stop()
        {
            _player.PlaybackSession.PlaybackRate = 0;
            _player.PlaybackSession.Position = TimeSpan.Zero;
            return Task.CompletedTask;
        }

        public async Task Play(string url)
        {
            _loadMediaTaskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                // Todo: sync this with the playback queue
                var mediaPlaybackList = new MediaPlaybackList();
                var mediaSource = MediaSource.CreateFromUri(new Uri(url));
                var item = new MediaPlaybackItem(mediaSource);
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

        private Task Play()
        {
            _player.PlaybackSession.PlaybackRate = 1;
            _player.Play();
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class AudioPlayerImplementation : IAudioPlayer
    {
        private readonly IVolumeManager _volumeManager;
        private readonly MediaPlayer _player;
        private readonly Timer _playProgressTimer;
        private PlaybackState _state;
        private IMediaItem _currentMediaFile;

        public AudioPlayerImplementation(IVolumeManager volumeManager)
        {
            _volumeManager = volumeManager;
            _player = new MediaPlayer();
            _playProgressTimer = new Timer(state =>
            {
                if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    var progress = _player.PlaybackSession.Position.TotalSeconds /
                                   _player.PlaybackSession.NaturalDuration.TotalSeconds;
                    if (double.IsInfinity(progress))
                        progress = 0;
                    Playing?.Invoke(this, new PlayingChangedEventArgs(progress, _player.PlaybackSession.Position, _player.PlaybackSession.NaturalDuration));
                }
            }, null, 0, int.MaxValue);

            _player.Failed += (sender, args) =>
                {
                    _state = MediaPlayerState.Failed;
                    _playProgressTimer.Change(0, int.MaxValue);
                    Failed?.Invoke(this, new MediaFailedEventArgs(args.ErrorMessage, args.ExtendedErrorCode));
                };

            _player.PlaybackSession.PlaybackStateChanged += (sender, args) =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.None:
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Opening:
                        State = MediaPlayerState.Loading;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Buffering:
                        State = MediaPlayerState.Buffering;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Playing:
                        if (sender.PlaybackRate <= 0 && sender.Position == TimeSpan.Zero)
                        {
                            State = MediaPlayerState.Stopped;
                        }
                        else
                        {
                            State = MediaPlayerState.Playing;
                            _playProgressTimer.Change(0, 50);
                        }
                        break;
                    case MediaPlaybackState.Paused:
                        State = MediaPlayerState.Paused;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            _player.MediaEnded += (sender, args) => { Finished?.Invoke(this, new MediaFinishedEventArgs(_currentMediaFile)); };

            _player.PlaybackSession.BufferingStarted += (sender, args) =>
            {
                var bufferedTime =
                    TimeSpan.FromSeconds(sender.BufferingProgress *
                                         sender.NaturalDuration.TotalSeconds);
                Buffering?.Invoke(this,
                    new BufferingChangedEventArgs(sender.BufferingProgress, bufferedTime));
            };

            _player.PlaybackSession.BufferingProgressChanged += (sender, args) =>
            {
                //This seems not to be fired at all
                var bufferedTime =
                    TimeSpan.FromSeconds(_player.PlaybackSession.BufferingProgress *
                                         _player.PlaybackSession.NaturalDuration.TotalSeconds);
                Buffering?.Invoke(this,
                    new BufferingChangedEventArgs(_player.PlaybackSession.BufferingProgress, bufferedTime));
            };

            _player.PlaybackSession.SeekCompleted += (sender, args) => { };
            int.TryParse((_player.Volume * 100).ToString(), out var vol);
            _volumeManager.CurrentVolume = vol;
            _volumeManager.Muted = _player.IsMuted;
            _volumeManager.VolumeChanged += VolumeManagerOnVolumeChanged;
        }

        private void VolumeManagerOnVolumeChanged(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            _player.Volume = (double)volumeChangedEventArgs.NewVolume;
            _player.IsMuted = volumeChangedEventArgs.Muted;
        }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public PlaybackState State
        {
            get { return _state; }
            private set
            {
                _state = value;
                Status?.Invoke(this, new StatusChangedEventArgs(_state));
            }
        }

        public event StatusChangedEventHandler Status;
        public event PlayingChangedEventHandler Playing;
        public event BufferingChangedEventHandler Buffering;
        public event MediaFinishedEventHandler Finished;
        public event MediaFailedEventHandler Failed;

        public TimeSpan Buffered
        {
            get
            {
                if (_player == null) return TimeSpan.Zero;
                return
                    TimeSpan.FromMilliseconds(_player.PlaybackSession.BufferingProgress *
                                              _player.PlaybackSession.NaturalDuration.TotalMilliseconds);
            }
        }

        public TimeSpan Duration => _player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;
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
            if ((_state == MediaPlayerState.Paused) || (_state == MediaPlayerState.Stopped))
                await Play();
            else
                await Pause();
        }

        public async Task Play(IMediaItem mediaFile = null)
        {
            try
            {
                var sameMediaFile = mediaFile == null || mediaFile.Equals(_currentMediaFile);
                var currentMediaPosition = _player.PlaybackSession?.Position;
                // This variable will determine whether you will resume your playback or not
                var resumeMediaFile = Status == MediaPlayerState.Paused && sameMediaFile ||
                                      currentMediaPosition?.TotalSeconds > 0 && sameMediaFile;
                if (resumeMediaFile)
                {
                    // TODO: PlaybackRate needs to be configurable rather than hard-coded here
                    //_player.PlaybackSession.PlaybackRate = 1;
                    _player.Play();
                    return;
                }

                if (mediaFile != null)
                {
                    _currentMediaFile = mediaFile;
                    var mediaPlaybackList = new MediaPlaybackList();
                    var mediaSource = await CreateMediaSource(mediaFile);
                    var item = new MediaPlaybackItem(mediaSource);
                    mediaPlaybackList.Items.Add(item);
                    _player.Source = mediaPlaybackList;
                    _player.Play();
                }
            }
            catch (Exception e)
            {
                Failed?.Invoke(this, new MediaFailedEventArgs("Unable to start playback", e));
                Status = MediaPlayerState.Stopped;
            }
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
            Status = MediaPlayerState.Stopped;
            return Task.CompletedTask;
        }

        private async Task<MediaSource> CreateMediaSource(IMediaItem mediaFile)
        {
            //switch (mediaFile.Availability)
            //{
            //    case ResourceAvailability.Remote:
            //        return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
            //    case ResourceAvailability.Local:
            //        var du = _player.SystemMediaTransportControls.DisplayUpdater;
            //        var storageFile = await StorageFile.GetFileFromPathAsync(mediaFile.Url);
            //        var playbackType = mediaFile.Type == MediaItemType.Audio
            //            ? MediaPlaybackType.Music
            //            : MediaPlaybackType.Video;
            //        await du.CopyFromFileAsync(playbackType, storageFile);
            //        du.Update();
            //        return MediaSource.CreateFromStorageFile(storageFile);
            //}

            return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
        }
    }
}
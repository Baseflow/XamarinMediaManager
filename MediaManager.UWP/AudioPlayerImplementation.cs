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
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    public class AudioPlayerImplementation : BasePlayerImplementation, IAudioPlayer
    {
        private readonly IVolumeManager _volumeManager;
        private readonly Timer _playProgressTimer;
        private MediaPlayerStatus _status;
        private IMediaFile _currentMediaFile;

        public AudioPlayerImplementation(IMediaPlyerPlaybackController mediaPlyerPlaybackController, IVolumeManager volumeManager)
            : base(mediaPlyerPlaybackController)
        {
            _volumeManager = volumeManager;
            _playProgressTimer = new Timer(state =>
            {
                if (Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    var progress = Player.PlaybackSession.Position.TotalSeconds /
                                   Player.PlaybackSession.NaturalDuration.TotalSeconds;
                    if (double.IsInfinity(progress))
                        progress = 0;
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(progress, Player.PlaybackSession.Position, Player.PlaybackSession.NaturalDuration));
                }
            }, null, 0, int.MaxValue);

            Player.MediaFailed += (sender, args) =>
                {
                    _status = MediaPlayerStatus.Failed;
                    _playProgressTimer.Change(0, int.MaxValue);
                    MediaFailed?.Invoke(this, new MediaFailedEventArgs(args.ErrorMessage, args.ExtendedErrorCode));
                };

            Player.PlaybackSession.PlaybackStateChanged += (sender, args) =>
            {
                switch (sender.PlaybackState)
                {
                    case MediaPlaybackState.None:
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Opening:
                        Status = MediaPlayerStatus.Loading;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Buffering:
                        Status = MediaPlayerStatus.Buffering;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    case MediaPlaybackState.Playing:
                        if (sender.PlaybackRate <= 0 && sender.Position == TimeSpan.Zero)
                        {
                            Status = MediaPlayerStatus.Stopped;
                        }
                        else
                        {
                            Status = MediaPlayerStatus.Playing;
                            _playProgressTimer.Change(0, 50);
                        }
                        break;
                    case MediaPlaybackState.Paused:
                        Status = MediaPlayerStatus.Paused;
                        _playProgressTimer.Change(0, int.MaxValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            Player.MediaEnded += (sender, args) => { MediaFinished?.Invoke(this, new MediaFinishedEventArgs(_currentMediaFile)); };

            Player.PlaybackSession.BufferingStarted += (sender, args) =>
            {
                var bufferedTime =
                    TimeSpan.FromSeconds(sender.BufferingProgress *
                                         sender.NaturalDuration.TotalSeconds);
                BufferingChanged?.Invoke(this,
                    new BufferingChangedEventArgs(sender.BufferingProgress, bufferedTime));
            };

            Player.PlaybackSession.BufferingProgressChanged += (sender, args) =>
            {
                //This seems not to be fired at all
                var bufferedTime =
                    TimeSpan.FromSeconds(Player.PlaybackSession.BufferingProgress *
                                         Player.PlaybackSession.NaturalDuration.TotalSeconds);
                BufferingChanged?.Invoke(this,
                    new BufferingChangedEventArgs(Player.PlaybackSession.BufferingProgress, bufferedTime));
            };

            Player.PlaybackSession.SeekCompleted += (sender, args) => { };
            int.TryParse((Player.Volume * 100).ToString(), out var vol);
            _volumeManager.CurrentVolume = vol;
            _volumeManager.Muted = Player.IsMuted;
            _volumeManager.VolumeChanged += VolumeManagerOnVolumeChanged;
        }

        private void VolumeManagerOnVolumeChanged(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            Player.Volume = (double)volumeChangedEventArgs.NewVolume;
            Player.IsMuted = volumeChangedEventArgs.Muted;
        }

        public Dictionary<string, string> RequestHeaders { get; set; }

        public MediaPlayerStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(_status));
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
                if (Player == null) return TimeSpan.Zero;
                return
                    TimeSpan.FromMilliseconds(Player.PlaybackSession.BufferingProgress *
                                              Player.PlaybackSession.NaturalDuration.TotalMilliseconds);
            }
        }

        public TimeSpan Duration => Player?.PlaybackSession.NaturalDuration ?? TimeSpan.Zero;
        public TimeSpan Position => Player?.PlaybackSession.Position ?? TimeSpan.Zero;

        public Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public async Task PlayPause()
        {
            if ((_status == MediaPlayerStatus.Paused) || (_status == MediaPlayerStatus.Stopped))
                await Play();
            else
                await Pause();
        }

        public async Task Play(IMediaFile mediaFile = null)
        {
            try
            {
                var sameMediaFile = mediaFile == null || mediaFile.Equals(_currentMediaFile);
                var currentMediaPosition = Player.PlaybackSession?.Position;
                // This variable will determine whether you will resume your playback or not
                var resumeMediaFile = Status == MediaPlayerStatus.Paused && sameMediaFile ||
                                      currentMediaPosition?.TotalSeconds > 0 && sameMediaFile;
                if (resumeMediaFile)
                {
                    // TODO: PlaybackRate needs to be configurable rather than hard-coded here
                    //Player.PlaybackSession.PlaybackRate = 1;
                    Player.Play();
                    return;
                }

                if (mediaFile != null)
                {
                    _currentMediaFile = mediaFile;
                    var mediaPlaybackList = new MediaPlaybackList();
                    var mediaSource = await CreateMediaSource(mediaFile);
                    var item = new MediaPlaybackItem(mediaSource);
                    mediaPlaybackList.Items.Add(item);
                    Player.Source = mediaPlaybackList;
                    Player.Play();
                }
            }
            catch (Exception e)
            {
                MediaFailed?.Invoke(this, new MediaFailedEventArgs("Unable to start playback", e));
                Status = MediaPlayerStatus.Stopped;
            }
        }

        public async Task Seek(TimeSpan position)
        {
            Player.PlaybackSession.Position = position;
            await Task.CompletedTask;
        }

        public Task Stop()
        {
            Player.PlaybackSession.PlaybackRate = 0;
            Player.PlaybackSession.Position = TimeSpan.Zero;
            Status = MediaPlayerStatus.Stopped;
            return Task.CompletedTask;
        }

        private async Task<MediaSource> CreateMediaSource(IMediaFile mediaFile)
        {
            switch (mediaFile.Availability)
            {
                case ResourceAvailability.Remote:
                    return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
                case ResourceAvailability.Local:
                    var du = Player.SystemMediaTransportControls.DisplayUpdater;
                    var storageFile = await StorageFile.GetFileFromPathAsync(mediaFile.Url);
                    var playbackType = mediaFile.Type == MediaFileType.Audio
                        ? MediaPlaybackType.Music
                        : MediaPlaybackType.Video;
                    await du.CopyFromFileAsync(playbackType, storageFile);
                    du.Update();
                    return MediaSource.CreateFromStorageFile(storageFile);
            }

            return MediaSource.CreateFromUri(new Uri(mediaFile.Url));
        }
    }
}
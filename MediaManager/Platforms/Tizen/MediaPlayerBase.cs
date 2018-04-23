using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Tizen.Multimedia;

namespace Plugin.MediaManager
{
    public abstract class MediaPlayerBase : IPlaybackManager
    {
        readonly IVolumeManager _volumeManager;
        readonly IMediaExtractor _mediaExtractor;
        readonly Timer _playProgressTimer;
        readonly Player _player;
        StreamInfo _streamInfo;
        MediaPlayerStatus _status;
        IMediaFile _currentMediaFile;
        int _lastRequestedSeekPosition;
        long _previousSeekTime = -1L;

        protected Player Player => _player;

        public MediaPlayerStatus Status
        {
            get => _status;
            protected set
            {
                _status = value;
                if (_player.State == PlayerState.Playing)
                {
                    _playProgressTimer.Change(0, 1000);
                }
                else
                {
                    _playProgressTimer.Change(0, int.MaxValue);
                }
                StatusChanged?.Invoke(this, new StatusChangedEventArgs(_status));
            }
        }

        public TimeSpan Position => IsStreamInfoAvailable() ? TimeSpan.FromMilliseconds(_player.GetPlayPosition()) : TimeSpan.Zero;

        public TimeSpan Duration => (_streamInfo != null && IsStreamInfoAvailable()) ? TimeSpan.FromMilliseconds(_streamInfo.GetDuration()) : TimeSpan.Zero;
        
        public TimeSpan Buffered => (_player?.State == PlayerState.Playing || _player?.State == PlayerState.Paused) ? TimeSpan.FromSeconds(_player.GetDownloadProgress().Current) : TimeSpan.Zero;

        public Dictionary<string, string> RequestHeaders { get; set; }

        public event StatusChangedEventHandler StatusChanged;

        public event PlayingChangedEventHandler PlayingChanged;

        public event BufferingChangedEventHandler BufferingChanged;

        public event MediaFinishedEventHandler MediaFinished;

        public event MediaFailedEventHandler MediaFailed;

        public MediaPlayerBase(IVolumeManager volumeManager, IMediaExtractor mediaExtractor)
        {
            _player = new Player();
            _volumeManager = volumeManager;
            ((VolumeManagerImplementation)_volumeManager)._player = _player;
            float.TryParse((_volumeManager.CurrentVolume / 100.0).ToString(), out var vol);
            _player.Volume = vol;

            _mediaExtractor = mediaExtractor;
            ((MediaExtractorImplementation)_mediaExtractor)._player = _player;

            _playProgressTimer = new Timer(callback =>
            {
                if (_player.State == PlayerState.Playing)
                {
                    var currentPosition = Position;
                    var currentDuration = Duration;
                    var progress = currentPosition.TotalSeconds / currentDuration.TotalSeconds;
                    if (double.IsInfinity(progress))
                        progress = 0;
                    PlayingChanged?.Invoke(this, new PlayingChangedEventArgs(progress, currentPosition, currentDuration));
                }
            }, null, 0, int.MaxValue);

           _player.ErrorOccurred += (sender, args) =>
            {
                Log.Debug("An error has occurred on Player : " + args.ToString());
                Status = MediaPlayerStatus.Stopped;
            };

            _player.PlaybackCompleted += (sender, args) =>
            {
                _playProgressTimer.Change(0, int.MaxValue);
                MediaFinished?.Invoke(this, new MediaFinishedEventArgs(_currentMediaFile));
            };

            _player.BufferingProgressChanged += (sender, args) =>
            {
                if (Status != MediaPlayerStatus.Buffering && _player?.State != PlayerState.Playing)
                {
                    Status = MediaPlayerStatus.Buffering;
                }
                BufferingChanged?.Invoke(this, new BufferingChangedEventArgs(args.Percent, Duration));
            };
        }

        public Task Pause()
        {
            if (_player.State == PlayerState.Playing)
            {
                _player.Pause();
                Status = MediaPlayerStatus.Paused;
            }
            return Task.CompletedTask;
        }

        public async Task Play(IMediaFile mediaFile = null)
        {
            if (mediaFile == null || (mediaFile != null && string.IsNullOrEmpty(mediaFile.Url)))
            {
                Log.Debug("Invalid media file to play : " + mediaFile );
                Status = MediaPlayerStatus.Stopped;
                return;
            }

            try
            {
                if (mediaFile.Equals(_currentMediaFile))
                {
                    if (_player.State == PlayerState.Playing)
                    {
                        _player.Stop();
                    }
                    else if (_player.State != PlayerState.Paused && _player.State != PlayerState.Ready)
                    {
                        Log.Debug("Invalid state to resume : " + _player.State);
                        Status = MediaPlayerStatus.Stopped;
                        return;
                    }
                    _player.Start();
                }
                else
                {
                    _currentMediaFile = mediaFile;
                    if (_player.State != PlayerState.Idle)
                        _player.Unprepare();

                    _player.SetSource(new MediaUriSource(mediaFile.Url));
                    PlayerInitialize();
                    await _player.PrepareAsync();
                    if (_player.State == PlayerState.Ready)
                    {
                        _streamInfo = _player.StreamInfo;
                        Status = MediaPlayerStatus.Loading;
                        _player.Start();
                    }
                }

                if (_player.State == PlayerState.Playing)
                {
                    Status = MediaPlayerStatus.Playing;
                }
                else
                {
                    Status = MediaPlayerStatus.Stopped;
                }
            }
            catch (Exception e)
            {
                Log.Debug("Fail to start player : " + e.ToString());
                Status = MediaPlayerStatus.Stopped;
            }
        }

        public async Task Seek(TimeSpan position)
        {
            int second = (int)position.TotalSeconds;
            if (_lastRequestedSeekPosition == second || _player == null)
                return;

            var nowTicks = DateTime.Now.Ticks;
            _lastRequestedSeekPosition = second;

            if (_previousSeekTime == -1L)
                _previousSeekTime = nowTicks;
            var diffInMilliseconds = (nowTicks - _previousSeekTime) / TimeSpan.TicksPerMillisecond;

            if (diffInMilliseconds < 1000)
                await Task.Delay(TimeSpan.FromMilliseconds(2000));

            _previousSeekTime = nowTicks;

            if (_lastRequestedSeekPosition != second)
                return;

            await _player.SetPlayPositionAsync(_lastRequestedSeekPosition, false);
        }

        public Task Stop()
        {
            if (_player.State == PlayerState.Playing || _player.State == PlayerState.Paused)
            {
                _player.Stop();
            }
            Status = MediaPlayerStatus.Stopped;
            return Task.CompletedTask;
        }

        protected virtual void PlayerInitialize()
        {
            _lastRequestedSeekPosition = -1;
            _previousSeekTime = -1L;
            _streamInfo = null;
        }

        private bool IsStreamInfoAvailable()
        {
            if (_player?.State == PlayerState.Ready || _player?.State == PlayerState.Playing || _player?.State == PlayerState.Paused)
                return true;
            else
                return false;
        }
    }
}
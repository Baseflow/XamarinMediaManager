using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Platforms.Uap;
using MediaManager.Platforms.Uap.Media;
using MediaManager.Platforms.Uap.Notifications;
using MediaManager.Platforms.Uap.Player;
using MediaManager.Platforms.Uap.Volume;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaManagerImplementation()
        {

        }

        private IMediaPlayer _mediaPlayer;
        public override IMediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                    _mediaPlayer = new WindowsMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public WindowsMediaPlayer WindowsdMediaPlayer => (WindowsMediaPlayer)MediaPlayer;

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager();
                return _volumeManager;
            }
            set => SetProperty(ref _volumeManager, value);
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                    _mediaExtractor = new MediaExtractor();
                return _mediaExtractor;
            }
            set => SetProperty(ref _mediaExtractor, value);
        }


        private INotificationManager _notificationManager;
        public override INotificationManager NotificationManager
        {
            get
            {
                if (_notificationManager == null)
                    _notificationManager = new NotificationManager();

                return _notificationManager;
            }
            set => SetProperty(ref _notificationManager, value);
        }

        public override Playback.MediaPlayerState State => WindowsdMediaPlayer.Player.PlaybackSession.PlaybackState.ToMediaPlayerState();

        public override TimeSpan Position => WindowsdMediaPlayer.Player.PlaybackSession.Position;

        public override TimeSpan Duration => WindowsdMediaPlayer.Player.PlaybackSession.NaturalDuration;

        public override TimeSpan Buffered => TimeSpan.FromMilliseconds(WindowsdMediaPlayer.Player.PlaybackSession.BufferingProgress);

        public override float Speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override RepeatMode RepeatMode {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override ShuffleMode ShuffleMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Init()
        {
            IsInitialized = true;
        }

        public override Task Play(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        public override Task<IMediaItem> Play(string uri)
        {
            throw new NotImplementedException();
        }

        public override Task Play(IEnumerable<IMediaItem> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            throw new NotImplementedException();
        }

        public override Task<IMediaItem> Play(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            throw new NotImplementedException();
        }

        public override Task Play()
        {
            return MediaPlayer.Play();
        }

        public override Task Pause()
        {
            return MediaPlayer.Pause();
        }

        public override Task Stop()
        {
            return MediaPlayer.Stop();
        }

        public override Task SeekTo(TimeSpan position)
        {
            return MediaPlayer.SeekTo(position);
        }
    }
}

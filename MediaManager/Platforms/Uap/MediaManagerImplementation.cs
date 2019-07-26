using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class MediaManagerImplementation : MediaManagerBase, IMediaManager<MediaPlayer>
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

        public WindowsMediaPlayer WindowsMediaPlayer => (WindowsMediaPlayer)MediaPlayer;

        public MediaPlayer Player => WindowsMediaPlayer.Player;

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

        public override TimeSpan Position => WindowsMediaPlayer?.Player?.PlaybackSession?.Position ?? TimeSpan.Zero;

        public override TimeSpan Duration => WindowsMediaPlayer?.Player?.PlaybackSession?.NaturalDuration ?? TimeSpan.Zero;

        public override float Speed
        {
            get
            {
                if (WindowsMediaPlayer?.Player?.PlaybackSession?.PlaybackRate == null)
                    return 0.0f;
                return ((float)WindowsMediaPlayer.Player.PlaybackSession.PlaybackRate);
            }
            set
            {
                if(WindowsMediaPlayer?.Player?.PlaybackSession?.PlaybackRate != null)
                    WindowsMediaPlayer.Player.PlaybackSession.PlaybackRate = value;
            }
        }

        public override RepeatMode RepeatMode
        {
            get
            {
                if (Player.IsLoopingEnabled)
                    return RepeatMode.All;
                else
                    return RepeatMode.Off;
            }
            set
            {
                switch (value)
                {
                    case RepeatMode.Off:
                        Player.IsLoopingEnabled = false;
                        break;
                    case RepeatMode.One:
                    case RepeatMode.All:
                        Player.IsLoopingEnabled = true;
                        break;
                }
            }
        }
    }
}

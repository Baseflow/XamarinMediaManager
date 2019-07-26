using System;
using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Platforms.Tizen.Media;
using MediaManager.Platforms.Tizen.Player;
using MediaManager.Platforms.Tizen.Volume;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Volume;
using TizenPlayer = Tizen.Multimedia.Player;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase, IMediaManager<TizenPlayer>
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
                    _mediaPlayer = new TizenMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public TizenMediaPlayer TizenMediaPlayer => (TizenMediaPlayer)MediaPlayer;
        public TizenPlayer Player => TizenMediaPlayer.Player;

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
                    _notificationManager = new MediaManager.Platforms.Tizen.Notifications.NotificationManager();

                return _notificationManager;
            }
            set => SetProperty(ref _notificationManager, value);
        }

        public override TimeSpan Position => TimeSpan.FromMilliseconds(Player.GetPlayPosition());

        public override TimeSpan Duration => TimeSpan.Zero;

        public override float Speed { get; set; }

        public override RepeatMode RepeatMode
        {
            get; set;
        }
    }
}

using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Platforms.Tizen.Media;
using MediaManager.Platforms.Tizen.Player;
using MediaManager.Platforms.Tizen.Volume;
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

        private IVolumeManager _volume;
        public override IVolumeManager Volume
        {
            get
            {
                if (_volume == null)
                    _volume = new VolumeManager();
                return _volume;
            }
            set => SetProperty(ref _volume, value);
        }

        private IMediaExtractor _extractor;
        public override IMediaExtractor Extractor
        {
            get
            {
                if (_extractor == null)
                    _extractor = new MediaExtractor();
                return _extractor;
            }
            set => SetProperty(ref _extractor, value);
        }


        private INotificationManager _notification;
        public override INotificationManager Notification
        {
            get
            {
                if (_notification == null)
                    _notification = new MediaManager.Platforms.Tizen.Notifications.NotificationManager();

                return _notification;
            }
            set => SetProperty(ref _notification, value);
        }

        public override TimeSpan Position => TimeSpan.FromMilliseconds(Player.GetPlayPosition());

        public override TimeSpan Duration => TimeSpan.Zero;

        public override float Speed { get; set; }

        public override bool KeepScreenOn
        {
            get;
            set;
        }
    }
}

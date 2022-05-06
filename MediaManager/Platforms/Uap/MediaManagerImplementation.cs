using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Platforms.Uap.Media;
using MediaManager.Platforms.Uap.Notifications;
using MediaManager.Platforms.Uap.Player;
using MediaManager.Platforms.Uap.Volume;
using MediaManager.Player;
using MediaManager.Volume;
using Windows.Media.Playback;
using Windows.System.Display;

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
                    _notification = new NotificationManager();

                return _notification;
            }
            set => SetProperty(ref _notification, value);
        }

        public override TimeSpan Position => WindowsMediaPlayer?.Player?.PlaybackSession?.Position ?? TimeSpan.Zero;

        public override TimeSpan Duration => WindowsMediaPlayer?.Player?.PlaybackSession?.NaturalDuration ?? TimeSpan.Zero;

        public override float Speed
        {
            get
            {
                if (WindowsMediaPlayer?.Player?.PlaybackSession?.PlaybackRate == null)
                    return 0.0f;
                return (float)WindowsMediaPlayer.Player.PlaybackSession.PlaybackRate;
            }
            set
            {
                if (WindowsMediaPlayer?.Player?.PlaybackSession?.PlaybackRate != null)
                    WindowsMediaPlayer.Player.PlaybackSession.PlaybackRate = value;
            }
        }

        protected DisplayRequest _displayRequest;
        protected bool _keepScreenOn;
        public override bool KeepScreenOn
        {
            get
            {
                return _keepScreenOn;
            }
            set
            {
                if (SetProperty(ref _keepScreenOn, value))
                {
                    if (_displayRequest == null)
                        _displayRequest = new DisplayRequest();

                    if (value)
                        _displayRequest.RequestActive();
                    else
                        _displayRequest.RequestRelease();
                }
            }
        }
    }
}

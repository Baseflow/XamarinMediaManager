using System.Runtime.InteropServices;
using System.Windows.Controls;
using MediaManager.Media;
using MediaManager.Notifications;
using MediaManager.Platforms.Wpf.Media;
using MediaManager.Platforms.Wpf.Notificiations;
using MediaManager.Platforms.Wpf.Player;
using MediaManager.Platforms.Wpf.Volume;
using MediaManager.Player;
using MediaManager.Volume;

namespace MediaManager
{
    public class MediaManagerImplementation : MediaManagerBase, IMediaManager<MediaElement>
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
                    _mediaPlayer = new WpfMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public WpfMediaPlayer WpfMediaPlayer => (WpfMediaPlayer)MediaPlayer;
        public MediaElement Player => WpfMediaPlayer.Player;

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

        public override TimeSpan Position => Player?.Position ?? TimeSpan.Zero;

        public override TimeSpan Duration => Player.NaturalDuration.HasTimeSpan ? Player.NaturalDuration.TimeSpan : TimeSpan.Zero;

        public override float Speed
        {
            get
            {
                return (float)Player.SpeedRatio;
            }
            set
            {
                Player.SpeedRatio = value;
            }
        }

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
                    if (value)
                        DisplayRequestActive();
                    else
                        DisplayRequestRelease();
                }
            }
        }

        protected void DisplayRequestActive()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.DISPLAY_REQUIRED | NativeMethods.EXECUTION_STATE.CONTINUOUS);
        }

        protected void DisplayRequestRelease()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.CONTINUOUS);
        }

        static class NativeMethods
        {
            [DllImport("Kernel32", SetLastError = true)]
            internal static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

            internal enum EXECUTION_STATE : uint
            {
                /// <summary>
                /// Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
                /// </summary>
                CONTINUOUS = 0x80000000,

                /// <summary>
                /// Forces the display to be on by resetting the display idle timer.
                /// </summary>
                DISPLAY_REQUIRED = 0x00000002,
            }
        }
    }
}

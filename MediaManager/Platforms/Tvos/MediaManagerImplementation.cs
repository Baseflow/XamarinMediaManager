using UIKit;

namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Tvos.Player.TvosMediaPlayer>
    {
        public MediaManagerImplementation()
        {
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
                    if (value && !UIApplication.SharedApplication.IdleTimerDisabled)
                        UIApplication.SharedApplication.IdleTimerDisabled = true;
                    else
                        UIApplication.SharedApplication.IdleTimerDisabled = false;
                }
            }
        }
    }
}

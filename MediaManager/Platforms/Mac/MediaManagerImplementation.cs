namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Mac.Player.MacMediaPlayer>
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
                    //TODO: Implement
                }
            }
        }
    }
}

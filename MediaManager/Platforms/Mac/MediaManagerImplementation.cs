namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Mac.Player.MacMediaPlayer>
    {
        public MediaManagerImplementation()
        {

        }
    }
}

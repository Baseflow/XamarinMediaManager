namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Tvos.Player.TvosMediaPlayer>
    {
        public MediaManagerImplementation()
        {
        }
    }
}

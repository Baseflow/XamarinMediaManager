namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Tvos.Media.TvosMediaPlayer>
    {
        public MediaManagerImplementation()
        {
        }
    }
}

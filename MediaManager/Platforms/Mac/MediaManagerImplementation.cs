using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Mac.Media.MediaPlayer>
    {
        public MediaManagerImplementation()
        {

        }
    }
}

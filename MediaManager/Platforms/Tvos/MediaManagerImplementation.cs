using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Video;
using MediaManager.Volume;
using MediaManager.Platforms.Tvos.Media;

namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Tvos.Media.MediaPlayer>
    {
        public MediaManagerImplementation()
        {
        }
    }
}

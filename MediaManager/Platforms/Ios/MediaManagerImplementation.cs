using MediaPlayer;
using UIKit;

namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Ios.Player.IosMediaPlayer>
    {
    }
}

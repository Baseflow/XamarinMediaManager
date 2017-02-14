using UIKit;

namespace Plugin.MediaManager
{
    public class VersionHelper : IVersionHelper
    {
        public bool SupportsAutomaticWaitPlayerProperty => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
    }
}

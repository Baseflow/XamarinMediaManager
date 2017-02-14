using Foundation;

namespace Plugin.MediaManager
{
    public class VersionHelper : IVersionHelper
    {
        private NSOperatingSystemVersion _version;

        public VersionHelper()
        {
            var processInfo = new NSProcessInfo();

            _version = processInfo.OperatingSystemVersion;
        }

        public bool SupportsAutomaticWaitPlayerProperty => _version.Major >= 10 && _version.Minor >= 12;
    }
}

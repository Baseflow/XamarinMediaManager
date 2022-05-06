using MediaManager.Media;

namespace MediaManager.Platforms.Wpf.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Wpf;

        public MediaExtractor()
        {
        }

        protected override Task<string> GetResourcePath(string resourceName)
        {
            return null;
        }
    }
}

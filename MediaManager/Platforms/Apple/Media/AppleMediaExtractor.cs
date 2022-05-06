using Foundation;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Media
{
    public class AppleMediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        public AppleMediaExtractor()
        {
        }

        public override IList<IMediaExtractorProvider> CreateProviders()
        {
            var providers = base.CreateProviders();
            providers.Add(new AVAssetProvider());
#if __IOS__ || __TVOS__
            providers.Add(new Ios.Media.AVAssetImageProvider());
#endif
            return providers;
        }

        protected override Task<string> GetResourcePath(string resourceName)
        {
            string path = null;

            var filename = Path.GetFileNameWithoutExtension(resourceName);
            var extension = Path.GetExtension(resourceName);

            path = NSBundle.MainBundle.PathForResource(filename, extension);

            return Task.FromResult(path);
        }
    }
}

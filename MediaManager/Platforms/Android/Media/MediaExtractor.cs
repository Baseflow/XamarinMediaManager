using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content.Res;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected Resources Resources => Resources.System;

        public MediaExtractor()
        {
        }

        public override IList<IMediaExtractorProvider> CreateProviders()
        {
            var providers = base.CreateProviders();
            providers.Add(new ID3Provider());
            providers.Add(new UriImageProvider());
            providers.Add(new FileImageProvider());
            providers.Add(new ResourceImageProvider());
            return providers;
        }

        protected override async Task<string> GetResourcePath(string resourceName)
        {
            string path = null;
            using (var stream = MediaManager.Context.Assets.Open(resourceName))
            {
                path = await CopyResourceStreamToFile(stream, "AndroidResources", resourceName).ConfigureAwait(false);
            }
            return path;
        }
    }
}

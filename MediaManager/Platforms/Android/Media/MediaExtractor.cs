using Android.Content.Res;
using Com.Google.Android.Exoplayer2.Upstream;
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

        protected override Task<string> GetResourcePath(string resourceName)
        {
            string path = null;
            try
            {
                if (int.TryParse(resourceName, out int resourceId))
                {
                    var rawDataSource = new RawResourceDataSource(MediaManager.Context);
                    var uri = RawResourceDataSource.BuildRawResourceUri(resourceId);
                    rawDataSource.Open(new DataSpec(uri));
                    path = rawDataSource.Uri.ToString();
                }
            }
            catch (Exception ex)
            {
                path = null;
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    var assetDataSource = new AssetDataSource(MediaManager.Context);
                    var dataSpec = new DataSpec(global::Android.Net.Uri.Parse(resourceName));
                    assetDataSource.Open(dataSpec);
                    path = assetDataSource.Uri.ToString();
                }
            }
            catch (Exception ex)
            {
                path = null;
                Console.WriteLine(ex.Message);
            }

            return Task.FromResult(path);
        }
    }
}

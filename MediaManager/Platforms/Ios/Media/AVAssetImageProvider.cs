using AVFoundation;
using Foundation;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Platforms.Apple.Media;
using UIKit;

namespace MediaManager.Platforms.Ios.Media
{
    public class AVAssetImageProvider : MediaExtractorProviderBase, IMediaItemImageProvider
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Apple;

        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            try
            {
                if (!string.IsNullOrEmpty(mediaItem.DisplayImageUri))
                {
                    var location = MediaManager.Extractor.GetMediaLocation(mediaItem.DisplayImageUri);
                    if (location == MediaLocation.Resource)
                        mediaItem.Image = image = UIImage.FromBundle(mediaItem.DisplayImageUri);
                    else
                        mediaItem.Image = image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(mediaItem.DisplayImageUri)));
                }
                if (image == null && !string.IsNullOrEmpty(mediaItem.AlbumImageUri))
                {
                    var location = MediaManager.Extractor.GetMediaLocation(mediaItem.AlbumImageUri);
                    if (location == MediaLocation.Resource)
                        mediaItem.AlbumImage = image = UIImage.FromBundle(mediaItem.AlbumImageUri);
                    else
                        mediaItem.AlbumImage = image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(mediaItem.AlbumImageUri)));
                }
                if (image == null)
                {
                    var assetsToLoad = new List<string>
                    {
                        AVMetadata.CommonKeyArtwork
                    };

                    var url = mediaItem.GetNSUrl();
                    var asset = AVAsset.FromUrl(url);
                    await asset.LoadValuesTaskAsync(assetsToLoad.ToArray()).ConfigureAwait(false);

                    var metadataDict = asset.CommonMetadata.ToDictionary(t => t.CommonKey, t => t);

                    image = UIImage.LoadFromData(metadataDict.GetValueOrDefault(AVMetadata.CommonKeyArtwork)?.DataValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return mediaItem.Image = image;
        }
    }
}

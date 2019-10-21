using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<object> ProvideImage(IMediaItem mediaItem)
        {
            object image = null;
            try
            {
                if (!string.IsNullOrEmpty(mediaItem.ImageUri))
                {
                    mediaItem.Image = image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(mediaItem.ImageUri)));
                }
                if (image == null && !string.IsNullOrEmpty(mediaItem.AlbumImageUri))
                {
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

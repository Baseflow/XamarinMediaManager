using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using MediaManager.Media;
#if __IOS__ || __TVOS__
using UIKit;
#endif

namespace MediaManager.Platforms.Apple.Media
{
    public class AppleMediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        public AppleMediaExtractor()
        {
        }

        public override async Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem)
        {
            var assetsToLoad = new List<string>
            {
                AVMetadata.CommonKeyArtist,
                AVMetadata.CommonKeyTitle,
                AVMetadata.CommonKeyArtwork
            };

            var url = GetUrlFor(mediaItem);

            // Default title to filename
            mediaItem.Title = url.LastPathComponent;

            var asset = AVAsset.FromUrl(url);
            await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

            foreach (var avMetadataItem in asset.CommonMetadata)
            {
                if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtist)
                {
                    mediaItem.Artist = ((NSString)avMetadataItem.Value).ToString();
                }
                else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyTitle)
                {
                    mediaItem.Title = ((NSString)avMetadataItem.Value).ToString();
                }
                else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtwork)
                {
#if __IOS__ || __TVOS__
                    var image = UIImage.LoadFromData(avMetadataItem.DataValue);
                    mediaItem.AlbumArt = image;
#endif
                }
            }

            
            return mediaItem;
        }

        public static NSUrl GetUrlFor(IMediaItem mediaItem)
        {
            var isLocallyAvailable = (mediaItem.MediaLocation == MediaLocation.FileSystem) || (mediaItem.MediaLocation == MediaLocation.Embedded);

            var url = isLocallyAvailable ? new NSUrl(mediaItem.MediaUri, false) : new NSUrl(mediaItem.MediaUri);

            return url;
        }

        public override Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }

        public override Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            var url = GetUrlFor(mediaItem);
            var imageGenerator = new AVAssetImageGenerator(AVAsset.FromUrl(url));
            imageGenerator.AppliesPreferredTrackTransform = true;
            var cgImage = imageGenerator.CopyCGImageAtTime(new CMTime((long)timeFromStart.TotalMilliseconds, 1000000), out var actualTime, out var error);
            return Task.FromResult(cgImage as object);
        }
    }
}

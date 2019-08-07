using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
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
                AVMetadata.CommonKeyAlbumName,
                AVMetadata.CommonKeyArtist,
                AVMetadata.CommonKeyAuthor,
                AVMetadata.CommonKeyCreationDate,
                AVMetadata.CommonKeyTitle,
                //AVMetadata.CommonKeyArtwork,
                //AVMetadata.CommonKeyCopyrights,
                //AVMetadata.CommonKeyDescription,
                //AVMetadata.CommonKeyFormat,
                //AVMetadata.CommonKeyLocation,
                //AVMetadata.CommonKeyLanguage,
                //AVMetadata.CommonKeyContributor,
                //AVMetadata.CommonKeyCreator,
                //AVMetadata.CommonKeyIdentifier,
                //AVMetadata.CommonKeyLastModifiedDate,
                //AVMetadata.CommonKeyMake,
                //AVMetadata.CommonKeySoftware,
                //AVMetadata.CommonKeyModel,
                //AVMetadata.CommonKeyPublisher,
                //AVMetadata.CommonKeyRelation,
                //AVMetadata.CommonKeySource,
                //AVMetadata.CommonKeySubject,
                //AVMetadata.CommonKeyType
            };

            var url = GetUrlFor(mediaItem);

            var asset = AVAsset.FromUrl(url);
            await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

            var metadataDict = asset.CommonMetadata.ToDictionary(t => t.CommonKey, t => t);

            if (string.IsNullOrEmpty(mediaItem.Album))
                mediaItem.Album = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyAlbumName)?.Value.ToString();

            if (string.IsNullOrEmpty(mediaItem.Artist))
                mediaItem.Artist = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyArtist)?.Value.ToString();

            if (string.IsNullOrEmpty(mediaItem.Author))
                mediaItem.Author = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyAuthor)?.Value.ToString();

            if (string.IsNullOrEmpty(mediaItem.Date))
                mediaItem.Date = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyCreationDate)?.Value.ToString();

            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyTitle)?.Value.ToString();

            // Default title to filename
            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = url.LastPathComponent;

            return mediaItem;
        }

        public static NSUrl GetUrlFor(IMediaItem mediaItem)
        {
            var isLocallyAvailable = mediaItem.MediaLocation.IsLocal();

            var url = isLocallyAvailable ? new NSUrl(mediaItem.MediaUri, false) : new NSUrl(mediaItem.MediaUri);

            return url;
        }

        public override async Task<object> GetMediaItemImage(IMediaItem mediaItem)
        {
            if (!string.IsNullOrEmpty(mediaItem.ArtUri))
            {
#if __IOS__ || __TVOS__
                var image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(mediaItem.ArtUri)));
                mediaItem.AlbumArt = image;

                return image;
#endif
            }
            else
            {
                var assetsToLoad = new List<string> 
                { 
                    AVMetadata.CommonKeyArtwork 
                };

                var url = GetUrlFor(mediaItem);
                var asset = AVAsset.FromUrl(url);
                await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

                var metadataDict = asset.CommonMetadata.ToDictionary(t => t.CommonKey, t => t);

#if __IOS__ || __TVOS__
                var image = UIImage.LoadFromData(metadataDict.GetValueOrDefault(AVMetadata.CommonKeyArtwork)?.DataValue);
                mediaItem.AlbumArt = image;

                return image;
#endif
            }

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

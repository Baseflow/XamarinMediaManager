using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Media
{
    public class AVAssetProvider : IMediaItemMetadataProvider, IMediaItemVideoFrameProvider
    {
        public AVAssetProvider()
        {
        }

        public async Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
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

            var url = mediaItem.GetNSUrl();

            var asset = AVAsset.FromUrl(url);
            await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

            var metadataDict = asset.CommonMetadata.ToDictionary(t => t.CommonKey, t => t);

            if (string.IsNullOrEmpty(mediaItem.Album))
                mediaItem.Album = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyAlbumName)?.Value.ToString();

            if (string.IsNullOrEmpty(mediaItem.Artist))
                mediaItem.Artist = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyArtist)?.Value.ToString();

            if (string.IsNullOrEmpty(mediaItem.Author))
                mediaItem.Author = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyAuthor)?.Value.ToString();

            var date = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyCreationDate)?.Value.ToString();
            if (mediaItem.Date == default && !string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dateResult))
                mediaItem.Date = dateResult;

            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = metadataDict.GetValueOrDefault(AVMetadata.CommonKeyTitle)?.Value.ToString();

            // Default title to filename
            if (string.IsNullOrEmpty(mediaItem.Title))
                mediaItem.Title = url.LastPathComponent;

            return mediaItem;
        }

        public Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            var url = mediaItem.GetNSUrl();
            var imageGenerator = new AVAssetImageGenerator(AVAsset.FromUrl(url));
            imageGenerator.AppliesPreferredTrackTransform = true;
            var cgImage = imageGenerator.CopyCGImageAtTime(new CMTime((long)timeFromStart.TotalMilliseconds, 1000000), out var actualTime, out var error);
            return Task.FromResult(cgImage as object);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Plugin.MediaManager.Abstractions;
#if __IOS__ || __TVOS__
using UIKit;
#endif

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            try
            {
                if (mediaFile.ExtractMetadata)
                {
                    var assetsToLoad = new List<string>
                    {
                        AVMetadata.CommonKeyArtist,
                        AVMetadata.CommonKeyTitle,
                        AVMetadata.CommonKeyArtwork
                    };

                    var url = MediaFileUrlHelper.GetUrlFor(mediaFile);

                    // Default title to filename
                    mediaFile.Metadata.Title = url.LastPathComponent;

                    var asset = AVAsset.FromUrl(url);
                    await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

                    foreach (var avMetadataItem in asset.CommonMetadata)
                    {
                        if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtist)
                        {
                            mediaFile.Metadata.Artist = ((NSString) avMetadataItem.Value).ToString();
                        }
                        else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyTitle)
                        {
                            mediaFile.Metadata.Title = ((NSString) avMetadataItem.Value).ToString();
                        }
                        else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtwork)
                        {
#if __IOS__ || __TVOS__
                            var image = UIImage.LoadFromData(avMetadataItem.DataValue);
                            mediaFile.Metadata.AlbumArt = image;
#endif
                        }
                    }
                    mediaFile.MetadataExtracted = true;
                }
                return mediaFile;
            }
            catch (Exception)
            {
                return mediaFile;
            }
        }
    }
}
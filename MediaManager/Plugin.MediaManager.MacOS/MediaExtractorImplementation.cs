using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
	public class MediaExtractorImplementation : IMediaExtractor
	{
		public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
		{
			try
			{
				var assetsToLoad = new List<string>
				{
					AVMetadata.CommonKeyArtist,
					AVMetadata.CommonKeyTitle,
					AVMetadata.CommonKeyArtwork
				};
				var nsUrl = new NSUrl(mediaFile.Url);

				// Default title to filename
				mediaFile.Title = nsUrl.LastPathComponent;

				var asset = AVAsset.FromUrl(nsUrl);
				await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

				foreach (var avMetadataItem in asset.CommonMetadata)
				{
					if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtist)
					{
						mediaFile.Artist = ((NSString)avMetadataItem.Value).ToString();
					}
					else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyTitle)
					{
						mediaFile.Title = ((NSString)avMetadataItem.Value).ToString();
					}
					else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtwork)
					{
						var image = NSObject.FromObject(avMetadataItem.DataValue);
						mediaFile.Cover = image;
					}
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
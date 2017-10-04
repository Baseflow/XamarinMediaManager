using System.Collections.Generic;

using Foundation;
using AVFoundation;

using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public static class MediaFileUrlHelper
    {
        public static NSUrl GetUrlFor(IMediaItem mediaFile) {
            var isLocallyAvailable = mediaFile.Availability == ResourceAvailability.Local;

            var url = isLocallyAvailable ? new NSUrl(mediaFile.Url, false) : new NSUrl(mediaFile.Url);

            return url;
        }

		public static AVUrlAssetOptions GetOptionsWithHeaders(IDictionary<string, string> headers)
		{
			var nativeHeaders = new NSMutableDictionary();

			foreach (var header in headers)
			{
				nativeHeaders.Add((NSString)header.Key, (NSString)header.Value);
			}

			var nativeHeadersKey = (NSString)"AVURLAssetHTTPHeaderFieldsKey";

			var options = new AVUrlAssetOptions(NSDictionary.FromObjectAndKey(
				nativeHeaders,
				nativeHeadersKey
			));

			return options;
		}
    }
}

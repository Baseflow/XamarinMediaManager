using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using Foundation;
using MediaManager.Media;

namespace MediaManager.Platforms.Apple.Media
{
    public static class MediaItemExtensions
    {
        public static Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public static AVPlayerItem GetPlayerItem(this IMediaItem mediaItem)
        {
            AVAsset asset;

            if (RequestHeaders != null && RequestHeaders.Any())
            {
                asset = AVUrlAsset.Create(NSUrl.FromString(mediaItem.MediaUri), GetOptionsWithHeaders(RequestHeaders));
            }
            else
            {
                asset = AVAsset.FromUrl(NSUrl.FromString(mediaItem.MediaUri));
            }

            var playerItem = AVPlayerItem.FromAsset(asset);

            return playerItem;
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

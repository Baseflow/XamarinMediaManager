using System;
using MediaManager.Media;

namespace MediaManager.AzureMediaServices.Models
{
    public class AzureMediaServiceItem : MediaItem
    {
        public AzureMediaServiceItem(string smoothStreamingUri, StreamingType defaultStreamingType = StreamingType.HLSv4) : base(smoothStreamingUri)
        {
            if (!smoothStreamingUri.EndsWith("manifest", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidAzureMediaServiceUriException($"{nameof(smoothStreamingUri)} must end with \"manifest\", e.g. https://amssamples.streaming.mediaservices.windows.net/91492735-c523-432b-ba01-faba6c2206a2/AzureMediaServicesPromo.ism/manifest");

            DefaultStreamingType = defaultStreamingType;
        }

        public string MPEGDashUri => MediaUri + "(format=mpd-time-csf)";
        public string HLSv3Uri => MediaUri + "(m3u8-aapl-v3)";
        public string HLSv4Uri => MediaUri + "(format=m3u8-aapl)";

        public StreamingType DefaultStreamingType { get; set; }
    }
}

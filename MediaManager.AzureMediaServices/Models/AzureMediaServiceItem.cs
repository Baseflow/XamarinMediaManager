using System;
using System.IO;

namespace MediaManager.AzureMediaServices.Models
{
    public class AzureMediaServiceItem
    {
        public AzureMediaServiceItem(string title, Stream media, string mediaId, DateTimeOffset expiryDate)
        {
            ExpiryDate = expiryDate;
            MediaId = mediaId;
            Title = title;
            Media = media;
        }

        public AzureMediaServiceItem(string title, Stream media, string mediaId) : this(title, media, mediaId, DateTimeOffset.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(mediaId))
                throw new ArgumentException($"{nameof(mediaId)} cannot be null or white space", nameof(mediaId));
        }

        public AzureMediaServiceItem(string title, Stream media) : this(title, media, Guid.NewGuid().ToString())
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"{nameof(title)} cannot be null or white space", nameof(title));

            if (media.Length <= 0)
                throw new ArgumentException($"{nameof(media)} cannot be empty", nameof(media));
        }

        public Stream Media { get; }
        public string MediaId { get; }
        public string Title { get; }
        public DateTimeOffset ExpiryDate { get; }

        public string MediaServicesAssetId { get; set; }
        public Uri MediaAssetUri { get; set; }
        public string AzureMediaServiceFileName { get; set; }
        public DateTimeOffset PublishedAt { get; set; }
        public string ManifestUrl { get; set; }
        public string HLSUrl { get; set; }
        public string MPEGDashUrl { get; set; }
        public string BlobStorageMediaUrl { get; set; }
    }
}

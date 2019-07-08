using System;

namespace MediaManager.AzureMediaServices.Models
{
    public class AzureMediaServiceItem
    {
        public AzureMediaServiceItem(string blobStorageMediaUrl, DateTimeOffset expiryDate, string fileName, Uri hlsUri, string id, Uri manifestUri, Uri mediaAssetUri, string mediaServicesAssetId, Uri mpegDashUri, DateTimeOffset publishedAt, string title) =>
            (BlobStorageMediaUrl, ExpiryDate, FileName, HLSUri, Id, ManifestUri, MediaAssetUri, MediaServicesAssetId, MPEGDashUri, PublishedAt, Title) = (blobStorageMediaUrl, expiryDate, fileName, hlsUri, id, manifestUri, mediaAssetUri, mediaServicesAssetId, mpegDashUri, publishedAt, title);

        public string BlobStorageMediaUrl { get; }
        public DateTimeOffset ExpiryDate { get; }
        public string FileName { get; }
        public Uri HLSUri { get; }
        public string Id { get; }
        public Uri ManifestUri { get; }
        public Uri MediaAssetUri { get; }
        public string MediaServicesAssetId { get; }
        public Uri MPEGDashUri { get; }
        public DateTimeOffset PublishedAt { get; }
        public string Title { get; }
    }
}

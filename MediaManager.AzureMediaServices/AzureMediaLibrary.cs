using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaManager.AzureMediaServices.Models;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaManager.AzureMediaServices
{
    public class AzureMediaLibrary
    {
        readonly AzureMediaService _azureMediaService;

        public AzureMediaLibrary(string azureMediaServiceTenantId, string azureMediaServiceClientId, string azureMediaServiceClientSecret, Uri azureMediaServiceEndpoint, string azureMediaServiceCdnProfileName = "", string azureMediaServiceCdnEndpointName = "")
        {
            _azureMediaService = new AzureMediaService(azureMediaServiceTenantId, azureMediaServiceClientId, azureMediaServiceClientSecret, azureMediaServiceEndpoint, azureMediaServiceCdnProfileName, azureMediaServiceCdnEndpointName);
        }

        public async Task<AzureMediaServiceItem> PublishMP4(string title, Stream mp4, DateTimeOffset expiryDate, CancellationToken cancellationToken)
        {
            IAsset unencodedAsset = null;
            IAsset encodedAsset = null;

            try
            {
                var id = Guid.NewGuid().ToString();
                var publishedAt = DateTimeOffset.UtcNow;

                cancellationToken.ThrowIfCancellationRequested();
                unencodedAsset = await _azureMediaService.CreateAssetAndUploadSingleFile(AssetCreationOptions.None, title, $"{id}_{publishedAt}.mp4", mp4, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                encodedAsset = await _azureMediaService.EncodeToAdaptiveBitrateMP4Set(unencodedAsset, title, cancellationToken).ConfigureAwait(false);

                var azureMediaServiceFileName = encodedAsset.Name;
                var mediaServicesAssetId = encodedAsset.Id;
                var mediaAssetUri = encodedAsset.Uri;

                cancellationToken.ThrowIfCancellationRequested();
                await _azureMediaService.CreateStreamingEndpoint().ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                var locator = await _azureMediaService.PublishMedia(encodedAsset, expiryDate.Subtract(DateTimeOffset.UtcNow)).ConfigureAwait(false);

                var (manifestUri, hlsUri, mpegDashUri) = _azureMediaService.BuildStreamingURLs(encodedAsset, locator);

                var blobStorageMediaUrl = $"{mediaAssetUri}/{_azureMediaService.GetMP4FileName(encodedAsset)}";

                return new AzureMediaServiceItem(blobStorageMediaUrl, expiryDate, azureMediaServiceFileName, hlsUri, id, manifestUri, mediaAssetUri, mediaServicesAssetId, mpegDashUri, publishedAt, title);
            }
            catch
            {
                await Task.WhenAll(unencodedAsset?.DeleteAsync(false) ?? Task.CompletedTask,
                                    encodedAsset?.DeleteAsync(false) ?? Task.CompletedTask).ConfigureAwait(false);
                throw;
            }
            finally
            {
                await (unencodedAsset?.DeleteAsync(false) ?? Task.CompletedTask).ConfigureAwait(false);
            }
        }
    }
}

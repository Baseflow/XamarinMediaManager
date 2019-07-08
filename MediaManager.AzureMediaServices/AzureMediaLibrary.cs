using System;
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

        public async Task<AzureMediaServiceItem> PublishMP4(AzureMediaServiceItem azureMediaServiceItem, CancellationToken cancellationToken)
        {
            IAsset unencodedAsset = null;
            IAsset encodedAsset = null;

            try
            {
                var currentTime = DateTimeOffset.UtcNow;
                azureMediaServiceItem.PublishedAt = currentTime;

                cancellationToken.ThrowIfCancellationRequested();
                unencodedAsset = await _azureMediaService.CreateAssetAndUploadSingleFile(AssetCreationOptions.None, azureMediaServiceItem.Title, $"{azureMediaServiceItem.MediaId}_{currentTime}.mp4", azureMediaServiceItem.Media, cancellationToken).ConfigureAwait(false);

                azureMediaServiceItem.MediaServicesAssetId = unencodedAsset.Id;
                azureMediaServiceItem.MediaAssetUri = unencodedAsset.Uri;

                cancellationToken.ThrowIfCancellationRequested();
                encodedAsset = await _azureMediaService.EncodeToAdaptiveBitrateMP4Set(unencodedAsset, azureMediaServiceItem.Title, cancellationToken).ConfigureAwait(false);

                azureMediaServiceItem.AzureMediaServiceFileName = encodedAsset.Name;
                azureMediaServiceItem.MediaServicesAssetId = encodedAsset.Id;
                azureMediaServiceItem.MediaAssetUri = encodedAsset.Uri;

                cancellationToken.ThrowIfCancellationRequested();
                await _azureMediaService.CreateStreamingEndpoint().ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                var locator = await _azureMediaService.PublishMedia(encodedAsset, azureMediaServiceItem.ExpiryDate.Subtract(DateTimeOffset.UtcNow)).ConfigureAwait(false);

                var (manifestUri, hlsUri, mpegDashUri) = _azureMediaService.BuildStreamingURLs(encodedAsset, locator);

                azureMediaServiceItem.BlobStorageMediaUrl = $"{azureMediaServiceItem.MediaAssetUri}/{_azureMediaService.GetMP4FileName(encodedAsset)}";
                azureMediaServiceItem.ManifestUrl = manifestUri;
                azureMediaServiceItem.HLSUrl = hlsUri;
                azureMediaServiceItem.MPEGDashUrl = mpegDashUri;

                return azureMediaServiceItem;
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

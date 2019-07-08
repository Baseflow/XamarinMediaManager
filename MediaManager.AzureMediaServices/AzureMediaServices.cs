using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.Live;

namespace MediaManager.AzureMediaServices
{
    class AzureMediaService
    {
        readonly string _azureMediaServiceCdnProfileName, _azureMediaServiceCdnEndpointName;
        readonly CloudMediaContext _cloudMediaContext;

        public AzureMediaService(string tenantId, string clientId, string clientSecret, Uri azureMediaServiceEndpoint, string azureMediaServiceCdnProfileName = "", string azureMediaServiceCdnEndpointName = "")
        {
            if (string.IsNullOrWhiteSpace(azureMediaServiceCdnProfileName))
                azureMediaServiceCdnProfileName = "MediaManagerCDNProfile";

            if (string.IsNullOrWhiteSpace(azureMediaServiceCdnEndpointName))
                azureMediaServiceCdnEndpointName = "MediaManagerCDNEndpoint";

            _azureMediaServiceCdnProfileName = azureMediaServiceCdnProfileName;
            _azureMediaServiceCdnEndpointName = azureMediaServiceCdnEndpointName;

            _cloudMediaContext = GetCloudMediaContext(tenantId, clientId, clientSecret, azureMediaServiceEndpoint);
        }

        public async Task<IAsset> EncodeToAdaptiveBitrateMP4Set(IAsset asset, string outputAssetName, CancellationToken cancellationToken)
        {
            const string encoderPreset = "Content Adaptive Multiple Bitrate MP4";
            const string processorName = "Media Encoder Standard";

            var job = _cloudMediaContext.Jobs.Create($"{processorName} Job");
            var processor = GetLatestMediaProcessorByName(processorName);

            var task = job.Tasks.AddNew($"Encoding {asset.Name}", processor, encoderPreset, TaskOptions.None);

            task.InputAssets.Add(asset);
            task.OutputAssets.AddNew(outputAssetName, AssetCreationOptions.None);

            await job.SubmitAsync().ConfigureAwait(false);
            await job.GetExecutionProgressTask(cancellationToken).ConfigureAwait(false);

            return job.OutputMediaAssets.FirstOrDefault();
        }

        public string GetMP4FileName(IAsset asset) =>
            Uri.EscapeDataString(asset.AssetFiles.Where(x => x.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Name)?.ToString();

        public async Task<IStreamingEndpoint> CreateStreamingEndpoint()
        {
            IStreamingEndpoint streamingEndpoint;

            if (!_cloudMediaContext.StreamingEndpoints.Any(x => x.CdnProfile.Equals(_azureMediaServiceCdnProfileName)))
            {
                var streamingEndpointOptions = new StreamingEndpointCreationOptions(_azureMediaServiceCdnEndpointName, 1)
                {
                    StreamingEndpointVersion = new Version("2.0"),
                    CdnEnabled = true,
                    CdnProfile = _azureMediaServiceCdnProfileName,
                    CdnProvider = CdnProviderType.StandardAkamai
                };

                streamingEndpoint = await _cloudMediaContext.StreamingEndpoints.CreateAsync(streamingEndpointOptions).ConfigureAwait(false);
            }
            else
            {
                streamingEndpoint = _cloudMediaContext.StreamingEndpoints.First(x => x.Name.Equals(_azureMediaServiceCdnEndpointName));
            }


            if (streamingEndpoint.State is StreamingEndpointState.Stopped)
                await streamingEndpoint.StartAsync().ConfigureAwait(false);

            return streamingEndpoint;
        }

        public Task<ILocator> PublishMedia(IAsset asset, TimeSpan publishTimeSpan)
        {
            var accessPolicy = _cloudMediaContext.AccessPolicies.Create(
                                                                "Streaming policy",
                                                                publishTimeSpan,
                                                                AccessPermissions.Read);

            return _cloudMediaContext.Locators.CreateLocatorAsync(
                                                LocatorType.OnDemandOrigin,
                                                asset,
                                                accessPolicy,
                                                DateTime.UtcNow.AddMinutes(-5));
        }

        public (Uri manifestUri, Uri hlsUri, Uri mpegDashUri) BuildStreamingURLs(IAsset asset, ILocator locator)
        {
            var manifestFile = asset.AssetFiles.Where(x => x.Name.ToLower().EndsWith(".ism", StringComparison.Ordinal)).FirstOrDefault();

            var manifestUrl = new Uri(GetStreamingManifestUrl(locator, manifestFile, _azureMediaServiceCdnEndpointName));

            var hlsUrl = new Uri($"{manifestUrl}(format=m3u8-aapl)");
            var dashUrl = new Uri($"{manifestUrl}(format=mpd-time-csf)");

            return (manifestUrl, hlsUrl, dashUrl);
        }


        public async Task<IAsset> CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string mediaTitle, string fileName, Stream mediaStream, CancellationToken token)
        {
            var inputAsset = await _cloudMediaContext.Assets.CreateAsync(mediaTitle, assetCreationOptions, token).ConfigureAwait(false);

            var assetFile = await inputAsset.AssetFiles.CreateAsync(fileName, token).ConfigureAwait(false);

            return await Task.Run(() =>
            {
                assetFile.Upload(mediaStream);

                return inputAsset;
            }).ConfigureAwait(false);
        }

        public List<IAsset> GetAssets() => _cloudMediaContext.Assets.ToList();

        public IAsset GetAsset(string id) => GetAssets().First(x => x.Id.Equals(id));

        CloudMediaContext GetCloudMediaContext(string tenantId, string clientId, string clientSecret, Uri mediaServiceEndpoint)
        {
            var tokenCredentials = new AzureAdTokenCredentials(tenantId,
                                                                new AzureAdClientSymmetricKey(clientId, clientSecret),
                                                                AzureEnvironments.AzureCloudEnvironment);

            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            return new CloudMediaContext(mediaServiceEndpoint, tokenProvider);
        }

        IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = _cloudMediaContext
                                .MediaProcessors
                                .Where(p => p.Name.Equals(mediaProcessorName))
                                .OrderBy(p => new Version(p.Version))
                                .LastOrDefault();

            return processor ?? throw new ArgumentException($"Unknown media processor: {mediaProcessorName}");
        }

        string GetStreamingManifestUrl(ILocator locator, IAssetFile assetFile, string cdnStreamingEndpointName)
        {
            var manifestUrl = locator.Path + assetFile.Name + "/manifest";
            manifestUrl = manifestUrl.Replace("http://", $"https://{cdnStreamingEndpointName}-");

            while (manifestUrl.Contains(" "))
                manifestUrl = manifestUrl.Replace(" ", "%20");

            return manifestUrl;
        }
    }
}

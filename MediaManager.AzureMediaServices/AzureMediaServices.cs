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
    class AzureMediaServices
    {
        #region Constant Fields
        const string encoderPreset = "Content Adaptive Multiple Bitrate MP4";
        readonly CloudMediaContext _cloudMediaContext;
        #endregion


        #region Constructors
        public AzureMediaServices(string tenantId, string clientId, string clientSecret, string azureMediaServiceEndpoint)
        {
            _cloudMediaContext = GetCloudMediaContext(tenantId, clientId, clientSecret, azureMediaServiceEndpoint);
        }
        #endregion

        #region Methods
        public async Task<IAsset> EncodeToAdaptiveBitrateMP4Set(IAsset asset, string outputAssetName, CancellationToken cancellationToken)
        {
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
            Uri.EscapeDataString(asset.AssetFiles.Where(x => x.Name.ToLower().EndsWith(".mp4", StringComparison.Ordinal)).FirstOrDefault()?.Name)?.ToString();

        public void CreateStreamingEndpoint(string cdnProfileName, string cdnStreamingEndpointName)
        {
            if (_cloudMediaContext.StreamingEndpoints.Where(x => x.CdnProfile.Equals(cdnProfileName)).Count() <= 0)
            {
                var streamingEndpointOptions = new StreamingEndpointCreationOptions(cdnStreamingEndpointName, 1)
                {
                    StreamingEndpointVersion = new Version("2.0"),
                    CdnEnabled = true,
                    CdnProfile = cdnProfileName,
                    CdnProvider = CdnProviderType.StandardAkamai,
                };

                _cloudMediaContext.StreamingEndpoints.Create(streamingEndpointOptions);
            }

            if (_cloudMediaContext.StreamingEndpoints.FirstOrDefault()?.State is StreamingEndpointState.Stopped)
                _cloudMediaContext.StreamingEndpoints.First().Start();
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

        public (string manifestUri, string hlsUri, string mpegDashUri) BuildStreamingURLs(IAsset asset, ILocator locator, string cdnStreamingEndpointName)
        {
            var manifestFile = asset.AssetFiles.Where(x => x.Name.ToLower().EndsWith(".ism", StringComparison.Ordinal)).FirstOrDefault();

            var manifestUrl = GetStreamingManifestUrl(locator, manifestFile, cdnStreamingEndpointName);

            var hlsUrl = $"{manifestUrl}(format=m3u8-aapl)";
            var dashUrl = $"{manifestUrl}(format=mpd-time-csf)";

            return (manifestUrl, hlsUrl, dashUrl);
        }


        public async Task<IAsset> CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string mediaTitle, string fileName, byte[] mediaFile, CancellationToken token)
        {
            var inputAsset = await _cloudMediaContext.Assets.CreateAsync(mediaTitle, assetCreationOptions, token).ConfigureAwait(false);

            var assetFile = await inputAsset.AssetFiles.CreateAsync(fileName, token).ConfigureAwait(false);

            return await Task.Run(() =>
            {
                using (var memoryStream = new MemoryStream(mediaFile))
                    assetFile.Upload(memoryStream);

                return inputAsset;
            }).ConfigureAwait(false);
        }

        public List<IAsset> GetAssets() => _cloudMediaContext.Assets.ToList();

        public IAsset GetAsset(string id) => GetAssets().First(x => x.Id.Equals(id));

        CloudMediaContext GetCloudMediaContext(string tenantId, string clientId, string clientSecret, string mediaServiceEndpoint)
        {
            var tokenCredentials = new AzureAdTokenCredentials(tenantId,
                                                                new AzureAdClientSymmetricKey(clientId, clientSecret),
                                                                AzureEnvironments.AzureCloudEnvironment);

            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            return new CloudMediaContext(new Uri(mediaServiceEndpoint), tokenProvider);
        }

        IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = _cloudMediaContext
                                .MediaProcessors
                                .Where(p => p.Name.Equals(mediaProcessorName))
                                .ToList()
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
        #endregion
    }
}

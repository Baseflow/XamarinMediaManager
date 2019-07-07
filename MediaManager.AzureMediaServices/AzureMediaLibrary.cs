using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.AzureMediaServices
{
    public class AzureMediaLibrary : IMediaLibrary
    {
        readonly AzureMediaServices azureMediaServices;

        public AzureMediaLibrary(string tenantId, string clientId, string clientSecret, string azureMediaServiceEndpoint)
        {
            azureMediaServices = new AzureMediaServices(tenantId, clientId, clientSecret, azureMediaServiceEndpoint);
        }

        public Task<IMediaItem> GetItem(string mediaId)
        {
            var items = azureMediaServices.GetAsset(mediaId);

            throw new NotImplementedException();

        }

        public Task<IEnumerable<IMediaItem>> GetItems()
        {
            var assets = azureMediaServices.GetAssets();

            throw new NotImplementedException();
        }

        public Task<IMediaItem> SaveItem(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }
    }
}

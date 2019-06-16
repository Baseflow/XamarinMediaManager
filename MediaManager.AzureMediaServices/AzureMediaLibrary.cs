using System;
using System.Collections.Generic;
using System.Text;
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

        public Task<MediaItem> GetItem(string mediaId)
        {
            var items = azureMediaServices.GetAsset(mediaId);

        }

        public Task<List<MediaItem>> GetItems()
        {
            var assets = azureMediaServices.GetAssets();
        }

        public Task<MediaItem> SaveItem(MediaItem mediaItem)
        {
            throw new NotImplementedException();
        }
    }
}

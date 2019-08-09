using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public class NfoMetadataProvider : IMetadataProvider
    {
        public Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            return null;
        }
    }
}

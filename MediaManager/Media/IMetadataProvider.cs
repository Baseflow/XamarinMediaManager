using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IMetadataProvider : IProvider
    {
        Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem);
    }
}

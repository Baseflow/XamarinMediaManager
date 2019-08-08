using System.Threading.Tasks;

namespace MediaManager.Media
{
    public interface IMetadataProvider : IProvider
    {
        Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem);
    }
}

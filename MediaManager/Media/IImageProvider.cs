using System.Threading.Tasks;

namespace MediaManager.Media
{
    public interface IImageProvider : IProvider
    {
        Task<object> ProvideImage(IMediaItem mediaItem);
    }
}

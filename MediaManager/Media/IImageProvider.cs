using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IImageProvider : IProvider
    {
        Task<object> ProvideImage(IMediaItem mediaItem);
    }
}

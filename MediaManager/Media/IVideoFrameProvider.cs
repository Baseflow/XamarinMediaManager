using System;
using System.Threading.Tasks;

namespace MediaManager.Media
{
    public interface IVideoFrameProvider : IProvider
    {
        Task<object> ProvideVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);
    }
}

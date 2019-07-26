using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MediaManager.Media
{
    public interface IMediaExtractor
    {
        IList<string> RemotePrefixes { get; }
        IList<string> FilePrefixes { get; }

        Task<IMediaItem> CreateMediaItem(string url);

        Task<IMediaItem> CreateMediaItem(FileInfo file);

        Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem);

        Task<object> RetrieveMediaItemArt(IMediaItem mediaItem);

        MediaLocation GetMediaLocation(IMediaItem mediaItem);
    }
}

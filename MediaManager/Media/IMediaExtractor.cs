using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaManager.Media
{
    public interface IMediaExtractor
    {
        IList<string> RemotePrefixes { get; }
        IList<string> FilePrefixes { get; }

        Task<IMediaItem> CreateMediaItem(string url);

        Task<IMediaItem> CreateMediaItem(string resourceName, Assembly assembly);

        Task<IMediaItem> CreateMediaItemFromNativeResource(string resourceName);

        Task<IMediaItem> CreateMediaItem(FileInfo file);

        Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem);

        Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem);

        Task<object> RetrieveMediaItemArt(IMediaItem mediaItem);

        MediaLocation GetMediaLocation(IMediaItem mediaItem);

        Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);
    }
}

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

        IList<string> ResourcePrefixes { get; }

        Task<IMediaItem> CreateMediaItem(string url);

        Task<IMediaItem> CreateMediaItemFromAssembly(string resourceName, Assembly assembly = null);

        Task<IMediaItem> CreateMediaItemFromResource(string resourceName);

        Task<IMediaItem> CreateMediaItem(FileInfo file);

        Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem);

        Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem);

        Task<object> RetrieveMediaItemArt(IMediaItem mediaItem);

        MediaLocation GetMediaLocation(IMediaItem mediaItem);

        Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);
    }
}

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
        IList<string> VideoSuffixes { get; }
        IList<string> AudioSuffixes { get; }
        IList<string> HlsSuffixes { get; }
        IList<string> SmoothStreamingSuffixes { get; }
        IList<string> DashSuffixes { get; }

        Task<IMediaItem> CreateMediaItem(string url);

        Task<IMediaItem> CreateMediaItemFromAssembly(string resourceName, Assembly assembly = null);

        Task<IMediaItem> CreateMediaItemFromResource(string resourceName);

        Task<IMediaItem> CreateMediaItem(FileInfo file);

        Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem);

        Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem);

        Task<object> GetMediaItemImage(IMediaItem mediaItem);

        Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);

        MediaLocation GetMediaLocation(IMediaItem mediaItem);

        MediaType GetMediaType(IMediaItem mediaItem);
    }
}

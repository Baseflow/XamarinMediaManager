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

        IList<IProvider> Providers { get; }
        IEnumerable<IMetadataProvider> MetadataProviders { get; }
        IEnumerable<IImageProvider> ImageProviders { get; }
        IEnumerable<IVideoFrameProvider> VideoFrameProviders { get; }

        Task<IMediaItem> CreateMediaItem(string url);

        Task<IMediaItem> CreateMediaItemFromAssembly(string resourceName, Assembly assembly = null);

        Task<IMediaItem> CreateMediaItemFromResource(string resourceName);

        Task<IMediaItem> CreateMediaItem(FileInfo file);

        Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem);

        Task<IMediaItem> GetMetadata(IMediaItem mediaItem);

        Task<object> GetMediaImage(IMediaItem mediaItem);

        Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);

        MediaLocation GetMediaLocation(IMediaItem mediaItem);

        MediaType GetMediaType(IMediaItem mediaItem);
    }
}

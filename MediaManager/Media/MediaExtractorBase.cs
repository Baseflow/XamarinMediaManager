using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MediaManager.Media
{
    public abstract class MediaExtractorBase : IMediaExtractor
    {
        protected Dictionary<string, string> RequestHeaders => CrossMediaManager.Current.RequestHeaders;

        public virtual Task<IMediaItem> CreateMediaItem(string url)
        {
            var mediaItem = new MediaItem(url);
            return UpdateMediaItem(mediaItem);
        }

        public virtual Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            return CreateMediaItem(file.FullName);
        }

        public virtual async Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem)
        {
            if (!mediaItem.IsMetadataExtracted)
            {
                mediaItem.MediaLocation = GetMediaLocation(mediaItem);
                mediaItem = await ExtractMetadata(mediaItem).ConfigureAwait(false);
            }

            return mediaItem;
        }

        public abstract Task<object> RetrieveMediaItemArt(IMediaItem mediaItem);

        public abstract Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem);

        public IList<string> RemotePrefixes { get; } = new List<string>() {
            "http",
            "udp",
            "rtp"
        };

        public IList<string> FilePrefixes { get; } = new List<string>() {
            "file",
            "/",
            "ms-appx",
            "android.resource"
        };

        public virtual MediaLocation GetMediaLocation(IMediaItem mediaItem)
        {
            var url = mediaItem.MediaUri.ToLower();
            foreach (var item in RemotePrefixes)
            {
                if (url.StartsWith(item))
                {
                    return MediaLocation.Remote;
                }
            }

            foreach (var item in FilePrefixes)
            {
                if (url.StartsWith(item))
                {
                    return MediaLocation.FileSystem;
                }
            }

            if (url.Length > 1 && url[1] == ':')
            {
                return MediaLocation.FileSystem;
            }

            return MediaLocation.Unknown;
        }
    }
}

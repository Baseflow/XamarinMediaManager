using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public virtual async Task<IMediaItem> CreateMediaItem(string resourceName, Assembly assembly)
        {
            string path = null;
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames
                .Where(x => x.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            using (var stream = assembly.GetManifestResourceStream(resourcePaths.Single()))
            {
                if (stream != null)
                {
                    var tempDirectory = Path.Combine(Path.GetTempPath(), "EmbeddedResources");
                    path = Path.Combine(tempDirectory, resourceName);

                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }

                    using (var tempFile = File.Create(path))
                    {
                        await stream.CopyToAsync(tempFile).ConfigureAwait(false);
                    }
                }
            }

            var mediaItem = new MediaItem(path);
            mediaItem.MediaLocation = MediaLocation.Embedded;
            return await UpdateMediaItem(mediaItem).ConfigureAwait(false);
        }

        public virtual Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            return CreateMediaItem(file.FullName);
        }

        public virtual async Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem)
        {
            if (!mediaItem.IsMetadataExtracted)
            {
                if (mediaItem.MediaLocation == MediaLocation.Unknown)
                {
                    mediaItem.MediaLocation = GetMediaLocation(mediaItem);
                }

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
            "ms-appdata",
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

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

        public IList<string> RemotePrefixes { get; } = new List<string>() {
            "http",
            "udp",
            "rtp"
        };

        public IList<string> FilePrefixes { get; } = new List<string>() {
            "file",
            "/",
            "ms-appx",
            "ms-appdata"
        };

        public IList<string> ResourcePrefixes { get; } = new List<string>() {
            "android.resource"
        };

        public IList<string> VideoSuffixes { get; } = new List<string>() {
            ".mp4"
        };

        public IList<string> AudioSuffixes { get; } = new List<string>() {
            ".mp3"
        };

        public IList<string> HlsSuffixes { get; } = new List<string>() {
            ".m3u8"
        };

        public IList<string> SmoothStreamingSuffixes { get; } = new List<string>() {
            ".ism",
            ".ism/manifest"
        };

        public IList<string> DashSuffixes { get; } = new List<string>() {
            ".mpd"
        };

        public virtual Task<IMediaItem> CreateMediaItem(string url)
        {
            var mediaItem = new MediaItem(url);
            return UpdateMediaItem(mediaItem);
        }

        public virtual Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            return CreateMediaItem(file.FullName);
        }

        public virtual async Task<IMediaItem> CreateMediaItemFromAssembly(string resourceName, Assembly assembly = null)
        {
            if (assembly == null)
            {
                if (!TryFindAssembly(resourceName, out assembly))
                    return null;
            }

            string path = null;
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames
                .Where(x => x.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (resourcePaths.Length < 1)
                return null;

            using (var stream = assembly.GetManifestResourceStream(resourcePaths.Single()))
            {
                path = await CopyResourceStreamToFile(stream, "EmbeddedResources", resourceName).ConfigureAwait(false);
            }

            var mediaItem = new MediaItem(path)
            {
                MediaLocation = MediaLocation.Embedded
            };
            return await UpdateMediaItem(mediaItem).ConfigureAwait(false);
        }

        public virtual async Task<IMediaItem> CreateMediaItemFromResource(string resourceName)
        {
            var path = await GetResourcePath(resourceName).ConfigureAwait(false);

            var mediaItem = new MediaItem(path)
            {
                MediaLocation = MediaLocation.Resource
            };
            return await UpdateMediaItem(mediaItem).ConfigureAwait(false);
        }

        public virtual async Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem)
        {
            if (!mediaItem.IsMetadataExtracted)
            {
                if (mediaItem.MediaLocation == MediaLocation.Unknown)
                {
                    mediaItem.MediaLocation = GetMediaLocation(mediaItem);
                }
                if(mediaItem.MediaType == MediaType.Default)
                {
                    mediaItem.MediaType = GetMediaType(mediaItem);
                }

                mediaItem = await ExtractMetadata(mediaItem).ConfigureAwait(false);
            }

            return mediaItem;
        }

        public abstract Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart);

        protected abstract Task<string> GetResourcePath(string resourceName);

        public abstract Task<object> RetrieveMediaItemArt(IMediaItem mediaItem);

        public abstract Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem);

        public virtual MediaType GetMediaType(IMediaItem mediaItem)
        {
            var url = mediaItem.MediaUri.ToLower();

            foreach (var item in VideoSuffixes)
            {
                if (url.EndsWith(item))
                    return MediaType.Video;
            }

            foreach (var item in AudioSuffixes)
            {
                if (url.EndsWith(item))
                    return MediaType.Audio;
            }

            foreach (var item in HlsSuffixes)
            {
                if (url.EndsWith(item))
                    return MediaType.Hls;
            }

            foreach (var item in DashSuffixes)
            {
                if (url.EndsWith(item))
                    return MediaType.Dash;
            }

            foreach (var item in SmoothStreamingSuffixes)
            {
                if (url.EndsWith(item))
                    return MediaType.SmoothStreaming;
            }

            return MediaType.Default;
        }

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

            foreach (var item in ResourcePrefixes)
            {
                if (url.StartsWith(item))
                {
                    return MediaLocation.Resource;
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

        protected virtual async Task<string> CopyResourceStreamToFile(Stream stream, string tempDirectoryName, string resourceName)
        {
            string path = null;

            if (stream != null)
            {
                var tempDirectory = Path.Combine(Path.GetTempPath(), tempDirectoryName);
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

            return path;
        }

        protected virtual bool TryFindAssembly(string resourceName, out Assembly assembly)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in assemblies)
            {
                var isResourceNameInAssembly = item.GetManifestResourceNames()
                    .Any(x => x.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));
                if (isResourceNameInAssembly)
                {
                    assembly = item;
                    return true;
                }
            }

            assembly = null;
            return false;
        }
    }
}

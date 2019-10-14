using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediaManager.Library;

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
            "android.resource",
            "raw",
            "assets"
        };

        public IList<string> VideoSuffixes { get; } = new List<string>() {
            ".3gp", ".3g2", ".asf", ".wmv", ".avi", ".divx", ".evo", ".f4v", ".flv", ".mkv", ".mk3d", ".mp4", ".mpg", ".mpeg", ".m2p", ".ps", ".ts", ".m2ts", ".mxf", ".ogg", ".mov", ".qt", ".rmvb", ".vob", ".webm"
        };

        public IList<string> AudioSuffixes { get; } = new List<string>() {
            ".3gp", ".aa", ".aac", ".aax", ".act", ".aiff", ".amr", ".ape", ".au", ".awb", ".dct", ".dss", ".dvf", ".flac", ".gsm", ".iklax", ".ivs", ".m4a", ".m4b", ".m4p", ".mmf", ".mp3", ".mpc", ".msv", ".nmf", ".nsf", ".ogg", ".oga,", ".mogg", ".opus", ".ra", ".rm", ".raw", ".sln", ".tta", ".voc", ".vox", ".wav", ".wma", ".wv", ".webm", ".8svx"
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

        private IList<IMediaExtractorProvider> _providers;
        public IList<IMediaExtractorProvider> Providers
        {
            get
            {
                if (_providers == null)
                    return Providers = CreateProviders();
                return _providers;
            }
            internal set => _providers = value;
        }

        public IEnumerable<IMediaItemMetadataProvider> MetadataProviders => Providers.OfType<IMediaItemMetadataProvider>();
        public IEnumerable<IMediaItemImageProvider> ImageProviders => Providers.OfType<IMediaItemImageProvider>();
        public IEnumerable<IMediaItemVideoFrameProvider> VideoFrameProviders => Providers.OfType<IMediaItemVideoFrameProvider>();

        public virtual IList<IMediaExtractorProvider> CreateProviders()
        {
            var providers = new List<IMediaExtractorProvider>();
            //providers.Add(new ApeTagProvider());
            //providers.Add(new NfoMetadataProvider());
            //providers.Add(new VorbisCommentProvider());
            //providers.Add(new XmlMetadataProvider());
            return providers;
        }

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
                if (string.IsNullOrEmpty(mediaItem.FileExtension))
                {
                    mediaItem.FileExtension = GetFileExtension(mediaItem);
                }
                if (mediaItem.MediaLocation == MediaLocation.Unknown)
                {
                    mediaItem.MediaLocation = GetMediaLocation(mediaItem);
                }
                if (mediaItem.MediaType == MediaType.Default)
                {
                    mediaItem.MediaType = GetMediaType(mediaItem);
                }
                mediaItem.DownloadStatus = GetDownloadStatus(mediaItem);

                mediaItem = await GetMetadata(mediaItem).ConfigureAwait(false);
                mediaItem.Image = await GetMediaImage(mediaItem).ConfigureAwait(false);
                mediaItem.IsMetadataExtracted = true;
            }

            return mediaItem;
        }

        public async Task<IMediaItem> GetMetadata(IMediaItem mediaItem)
        {
            foreach (var provider in MetadataProviders)
            {
                var item = await provider.ProvideMetadata(mediaItem).ConfigureAwait(false);
                if (item != null)
                    mediaItem = item;
            }
            return mediaItem;
        }

        public async Task<object> GetMediaImage(IMediaItem mediaItem)
        {
            object image = null;

            if (mediaItem.IsMetadataExtracted)
                image = mediaItem.GetImage();

            if (image == null)
            {
                foreach (var provider in ImageProviders)
                {
                    image = await provider.ProvideImage(mediaItem).ConfigureAwait(false);
                    if (image != null)
                        return image;
                }
            }
            return image;
        }

        public async Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            object image = null;
            foreach (var provider in VideoFrameProviders)
            {
                image = await provider.ProvideVideoFrame(mediaItem, timeFromStart).ConfigureAwait(false);
                if (image != null)
                    return image;
            }
            return image;
        }

        protected abstract Task<string> GetResourcePath(string resourceName);

        public virtual string GetFileExtension(IMediaItem mediaItem)
        {
            var url = mediaItem?.MediaUri?.ToLower();
            var suffixes = VideoSuffixes.Union(AudioSuffixes).Union(HlsSuffixes).Union(DashSuffixes).Union(SmoothStreamingSuffixes);

            //Try to find the best match
            foreach (var item in suffixes)
            {
                if (url.EndsWith(item))
                    return item;
            }
            //If no match available see if the url contains info
            foreach (var item in suffixes)
            {
                if (url.Contains(item))
                    return item;
            }
            return mediaItem?.FileExtension;
        }

        public virtual MediaType GetMediaType(IMediaItem mediaItem)
        {
            var fileExtension = !string.IsNullOrEmpty(mediaItem?.FileExtension)
                ? mediaItem.FileExtension
                : GetFileExtension(mediaItem);

            if (VideoSuffixes.Contains(fileExtension))
                return MediaType.Video;
            else if (AudioSuffixes.Contains(fileExtension))
                return MediaType.Audio;
            else if (HlsSuffixes.Contains(fileExtension))
                return MediaType.Hls;
            else if (DashSuffixes.Contains(fileExtension))
                return MediaType.Dash;
            else if (SmoothStreamingSuffixes.Contains(fileExtension))
                return MediaType.SmoothStreaming;

            return MediaType.Default;
        }

        public virtual MediaLocation GetMediaLocation(IMediaItem mediaItem)
        {
            var url = mediaItem?.MediaUri?.ToLower();

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

        public virtual DownloadStatus GetDownloadStatus(IMediaItem mediaItem)
        {
            switch (mediaItem.MediaLocation)
            {
                case MediaLocation.Unknown:
                case MediaLocation.Remote:
                    return DownloadStatus.NotDownloaded;
                case MediaLocation.FileSystem:
                case MediaLocation.Embedded:
                case MediaLocation.Resource:
                default:
                    return DownloadStatus.Downloaded;
            }
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
            foreach (var item in assemblies)
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

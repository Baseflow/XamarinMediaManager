﻿using System;
using System.Collections.Concurrent;
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

        private IList<IProvider> _providers;
        public IList<IProvider> Providers
        {
            get
            {
                if (_providers == null)
                    return Providers = CreateProviders();
                return _providers;
            }
            internal set => _providers = value;
        }

        public IEnumerable<IMetadataProvider> MetadataProviders => Providers.OfType<IMetadataProvider>();
        public IEnumerable<IImageProvider> ImageProviders => Providers.OfType<IImageProvider>();
        public IEnumerable<IVideoFrameProvider> VideoFrameProviders => Providers.OfType<IVideoFrameProvider>();

        public virtual IList<IProvider> CreateProviders()
        {
            var providers = new List<IProvider>();
            //TODO: Add some overall providers
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

        BlockingCollection<IMediaItem> _mediaItemsToExtract = new BlockingCollection<IMediaItem>(new ConcurrentQueue<IMediaItem>());

        Task _mediaItemsMonitor;

        public virtual Task<IMediaItem> UpdateMediaItem(IMediaItem mediaItem)
        {
            if (!mediaItem.IsMetadataExtracted)
            {
                if (mediaItem.MediaLocation == MediaLocation.Unknown)
                {
                    mediaItem.MediaLocation = GetMediaLocation(mediaItem);
                }
                if (mediaItem.MediaType == MediaType.Default)
                {
                    mediaItem.MediaType = GetMediaType(mediaItem);
                }

                if (_mediaItemsMonitor == null)
                    _mediaItemsMonitor = Task.Run(MonitorMediaItems);

                _mediaItemsToExtract.Add(mediaItem);
            }

            return Task.FromResult(mediaItem);
        }

        private async Task MonitorMediaItems()
        {
            while (true)
            {
                IMediaItem mediaItem = _mediaItemsToExtract.Take();
                mediaItem = await GetMetadata(mediaItem).ConfigureAwait(false);
                mediaItem.IsMetadataExtracted = true;
            }
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
            foreach (var provider in ImageProviders)
            {
                image = await provider.ProvideImage(mediaItem);
                if (image != null)
                    return image;
            }
            return image;
        }

        public async Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            object image = null;
            foreach (var provider in VideoFrameProviders)
            {
                image = await provider.ProvideVideoFrame(mediaItem, timeFromStart);
                if (image != null)
                    return image;
            }
            return image;
        }

        protected abstract Task<string> GetResourcePath(string resourceName);

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

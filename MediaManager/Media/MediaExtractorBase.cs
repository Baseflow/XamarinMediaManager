using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MediaManager.Media
{
    public abstract class MediaExtractorBase : IMediaExtractor
    {
        public virtual async Task<IMediaItem> CreateMediaItem(string url)
        {
            var mediaItem = new MediaItem(url);
            mediaItem.MediaLocation = GetMediaLocation(mediaItem);
            return await ExtractMetadata(mediaItem);
        }

        public virtual async Task<IMediaItem> CreateMediaItem(FileInfo file)
        {
            var mediaItem = new MediaItem(file.FullName);
            mediaItem.MediaLocation = GetMediaLocation(mediaItem);
            return await ExtractMetadata(mediaItem);
        }

        public virtual async Task<IMediaItem> CreateMediaItem(IMediaItem mediaItem)
        {
            mediaItem.MediaLocation = GetMediaLocation(mediaItem);
            return await ExtractMetadata(mediaItem);
        }

        public virtual Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }

        public abstract Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem);

        public virtual MediaLocation GetMediaLocation(IMediaItem mediaItem)
        {
            if (mediaItem.MediaUri.StartsWith("http")) return MediaLocation.Remote;
            if (mediaItem.MediaUri.StartsWith("file") 
                || mediaItem.MediaUri.StartsWith("/") 
                || (mediaItem.MediaUri.Length > 1 && mediaItem.MediaUri[1] == ':')) return MediaLocation.FileSystem;

            return MediaLocation.Unknown;
        }
    }
}

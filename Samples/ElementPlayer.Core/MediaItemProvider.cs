using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElementPlayer.Core.Assets;
using MediaManager;
using MediaManager.Library;
using MediaManager.Media;

namespace ElementPlayer.Core
{
    public class MediaItemProvider : IMediaItemProvider
    {
        public bool Enabled { get; set; } = true;

        public Task<bool> AddOrUpdate(IMediaItem item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IMediaItem> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IMediaItem>> GetAll()
        {
            var json = ExoPlayerSamples.GetEmbeddedResourceString("media.exolist.json");
            var list = ExoPlayerSamples.FromJson(json);

            var items = new List<IMediaItem>();

            foreach (var item in list)
            {
                foreach (var sample in item.Samples)
                {
                    if (!string.IsNullOrEmpty(sample.Uri))
                    {
                        IMediaItem mediaItem = new MediaItem(sample.Uri) { IsMetadataExtracted = true };
                        mediaItem = await CrossMediaManager.Current.Extractor.UpdateMediaItem(mediaItem).ConfigureAwait(false);
                        //var mediaItem = new MediaItem(sample.Uri);
                        mediaItem.Title = sample.Name;
                        mediaItem.Album = item.Name;
                        mediaItem.FileExtension = sample.Extension;
                        if (mediaItem.FileExtension == "mpd" || mediaItem.MediaUri.EndsWith(".mpd"))
                            mediaItem.MediaType = MediaType.Dash;
                        else if (mediaItem.FileExtension == "ism" || mediaItem.MediaUri.EndsWith(".ism"))
                            mediaItem.MediaType = MediaType.SmoothStreaming;
                        else if (mediaItem.FileExtension == "m3u8" || mediaItem.MediaUri.EndsWith(".m3u8"))
                            mediaItem.MediaType = MediaType.Hls;
                        
                        if (mediaItem.MediaUri.StartsWith("inMemory:"))
                        {
                            mediaItem.Data = typeof(MediaItemProvider).Assembly.GetManifestResourceStream(mediaItem.MediaUri.Substring(9));
                            mediaItem.MediaUri = string.Empty;
                            mediaItem.MediaLocation = MediaLocation.InMemory;
                            mediaItem.MimeType = sample.MimeType.ToMimeType();
                        }

                        items.Add(mediaItem);
                    }
                }
            }
            return items;
        }

        public Task<bool> Remove(IMediaItem item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAll()
        {
            throw new NotImplementedException();
        }
    }
}

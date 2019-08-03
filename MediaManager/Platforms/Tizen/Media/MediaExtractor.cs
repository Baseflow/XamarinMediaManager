using System;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Media;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Tizen;

        public override Task<IMediaItem> ExtractMetadata(IMediaItem mediaItem)
        {
            var extractor = new MetadataExtractor(mediaItem.MediaUri);
            SetMetadata(mediaItem, extractor);
            return Task.FromResult(mediaItem);
        }

        public override Task<object> GetVideoFrame(IMediaItem mediaItem, TimeSpan timeFromStart)
        {
            return null;
        }

        public override Task<object> RetrieveMediaItemArt(IMediaItem mediaItem)
        {
            return null;
        }

        protected override Task<string> GetNativeResourcePath(string resourceName)
        {
            return null;
        }

        protected virtual void SetMetadata(IMediaItem mediaItem, MetadataExtractor extractor)
        {
            var metadata = extractor.GetMetadata();
            mediaItem.Title = metadata.Title;
            mediaItem.Artist = metadata.Artist;
            mediaItem.Album = metadata.Album;
            mediaItem.AlbumArtist = metadata.AlbumArtist;
            mediaItem.Author = metadata.Author;
            mediaItem.Duration = TimeSpan.FromSeconds(metadata?.Duration ?? 0);
            mediaItem.Genre = metadata.Genre;
            if (int.TryParse(metadata.TrackNumber, out var year))
            {
                mediaItem.TrackNumber = year;
                mediaItem.NumTracks = year;
            }

            var buffer = mediaItem.MediaType == MediaType.Video ? extractor.GetVideoThumbnail() : extractor.GetArtwork().Data;
            if (buffer.Length > 0)
            {
                Stream st = new MemoryStream(buffer);
                mediaItem.AlbumArt = st;
            }
        }

        protected virtual void SetMetadata(IMediaItem mediaItem, StreamInfo streamInfo)
        {
            mediaItem.Title = streamInfo.GetMetadata(StreamMetadataKey.Title);
            mediaItem.Artist = streamInfo.GetMetadata(StreamMetadataKey.Artist);
            mediaItem.AlbumArtist = streamInfo.GetMetadata(StreamMetadataKey.Album);
            mediaItem.Author = streamInfo.GetMetadata(StreamMetadataKey.Author);
            mediaItem.Duration = TimeSpan.FromSeconds(streamInfo.GetDuration());
            mediaItem.Genre = streamInfo.GetMetadata(StreamMetadataKey.Genre);
            if (long.TryParse(streamInfo.GetMetadata(StreamMetadataKey.Year), out var year))
            {
                mediaItem.Year = Convert.ToInt32(year);
            }
            mediaItem.AlbumArt = streamInfo.GetAlbumArt();
        }
    }
}

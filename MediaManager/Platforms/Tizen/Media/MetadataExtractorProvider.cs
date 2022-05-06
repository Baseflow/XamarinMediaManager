using MediaManager.Library;
using MediaManager.Media;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Media
{
    public class MetadataExtractorProvider : MediaExtractorProviderBase, IMediaItemMetadataProvider, IMediaItemImageProvider
    {
        public Task<IMediaItem> ProvideMetadata(IMediaItem mediaItem)
        {
            var extractor = new MetadataExtractor(mediaItem.MediaUri);
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
            return Task.FromResult(mediaItem);
        }

        public Task<object> ProvideImage(IMediaItem mediaItem)
        {
            var extractor = new MetadataExtractor(mediaItem.MediaUri);
            var buffer = mediaItem.MediaType == MediaType.Video ? extractor.GetVideoThumbnail() : extractor.GetArtwork().Data;
            Stream stream = null;
            if (buffer.Length > 0)
            {
                stream = new MemoryStream(buffer);
            }
            return Task.FromResult(stream as object);
        }
    }
}

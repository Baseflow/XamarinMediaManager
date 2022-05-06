using MediaManager.Media;

namespace MediaManager.Platforms.Tizen.Media
{
    public class MediaExtractor : MediaExtractorBase, IMediaExtractor
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Tizen;

        public MediaExtractor()
        {
        }

        public override IList<IMediaExtractorProvider> CreateProviders()
        {
            var providers = base.CreateProviders();
            providers.Add(new MetadataExtractorProvider());
            return providers;
        }

        protected override Task<string> GetResourcePath(string resourceName)
        {
            return null;
        }

        //TODO: Move to streaminfo provider
        /*
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
        }*/
    }
}

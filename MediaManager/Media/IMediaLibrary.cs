using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IMediaLibrary
    {
        bool ReturnOnFirstResult { get; set; }

        IList<ILibraryProvider> Providers { get; }

        IEnumerable<IPlaylistProvider> PlaylistProviders { get; }
        IEnumerable<IArtistProvider> ArtistProviders { get; }
        IEnumerable<IAlbumProvider> AlbumProviders { get; }
        IEnumerable<IRadioProvider> RadioProviders { get; }
        IEnumerable<IMediaItemProvider> MediaItemProviders { get; }

        Task<IEnumerable<TContentItem>> GetAll<TContentItem>() where TContentItem : IContentItem;
        Task<TContentItem> Get<TContentItem>(string id) where TContentItem : IContentItem;
        Task<bool> Exists<TContentItem>(string id) where TContentItem : IContentItem;
        Task<bool> AddOrUpdate<TContentItem>(TContentItem item) where TContentItem : IContentItem;
        Task<bool> Remove<TContentItem>(TContentItem item) where TContentItem : IContentItem;
        Task<bool> RemoveAll<TContentItem>() where TContentItem : IContentItem;
    }
}

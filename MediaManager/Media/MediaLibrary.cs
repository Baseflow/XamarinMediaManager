using MediaManager.Library;

namespace MediaManager.Media
{
    public class MediaLibrary : IMediaLibrary
    {
        public MediaLibrary()
        {
        }

        //TODO: Implement to return always on first result find
        public bool ReturnOnFirstResult { get; set; }

        private IList<ILibraryProvider> _providers;
        public IList<ILibraryProvider> Providers
        {
            get
            {
                if (_providers == null)
                    return Providers = CreateProviders();
                return _providers;
            }
            internal set => _providers = value;
        }

        public virtual IList<ILibraryProvider> CreateProviders()
        {
            var providers = new List<ILibraryProvider>();
            return providers;
        }

        public IEnumerable<IPlaylistProvider> PlaylistProviders => Providers.OfType<IPlaylistProvider>();
        public IEnumerable<IArtistProvider> ArtistProviders => Providers.OfType<IArtistProvider>();
        public IEnumerable<IAlbumProvider> AlbumProviders => Providers.OfType<IAlbumProvider>();
        public IEnumerable<IRadioProvider> RadioProviders => Providers.OfType<IRadioProvider>();
        public IEnumerable<IMediaItemProvider> MediaItemProviders => Providers.OfType<IMediaItemProvider>();

        public async Task<IEnumerable<TContentItem>> GetAll<TContentItem>() where TContentItem : IContentItem
        {
            var items = new List<TContentItem>();
            IList<Task<IEnumerable<TContentItem>>> tasks = new List<Task<IEnumerable<TContentItem>>>();

            foreach (var provider in Providers.Where(x => x.Enabled).OfType<ILibraryProvider<TContentItem>>())
            {
                tasks.Add(provider.GetAll());
            }
            foreach (var item in await Task.WhenAll(tasks).ConfigureAwait(false))
            {
                items.AddRange(item);
            }

            return items;
        }

        public async Task<TContentItem> Get<TContentItem>(string id) where TContentItem : IContentItem
        {
            foreach (var provider in Providers.Where(x => x.Enabled).OfType<ILibraryProvider<TContentItem>>())
            {
                var item = await provider.Get(id).ConfigureAwait(false);
                if (item != null)
                    return item;
            }
            return default;
        }

        public async Task<bool> AddOrUpdate<TContentItem>(TContentItem item) where TContentItem : IContentItem
        {
            foreach (var provider in Providers.Where(x => x.Enabled).OfType<ILibraryProvider<TContentItem>>())
            {
                var success = await provider.AddOrUpdate(item).ConfigureAwait(false);
                if (success)
                    return success;
            }
            return false;
        }

        public async Task<bool> Remove<TContentItem>(TContentItem item) where TContentItem : IContentItem
        {
            foreach (var provider in Providers.Where(x => x.Enabled).OfType<ILibraryProvider<TContentItem>>())
            {
                var success = await provider.Remove(item).ConfigureAwait(false);
                if (success)
                    return success;
            }
            return false;
        }

        public async Task<bool> RemoveAll<TContentItem>() where TContentItem : IContentItem
        {
            IList<Task<bool>> tasks = new List<Task<bool>>();
            foreach (var provider in Providers.Where(x => x.Enabled).OfType<ILibraryProvider<TContentItem>>())
            {
                tasks.Add(provider.RemoveAll());
            }
            var success = await Task.WhenAll(tasks).ConfigureAwait(false);
            return success.All(x => x == true);
        }

        public async Task<bool> Exists<TContentItem>(string id) where TContentItem : IContentItem
        {
            foreach (var provider in Providers.Where(x => x.Enabled).OfType<ILibraryProvider<TContentItem>>())
            {
                var success = await provider.Exists(id).ConfigureAwait(false);
                if (success)
                    return success;
            }
            return false;
        }
    }
}

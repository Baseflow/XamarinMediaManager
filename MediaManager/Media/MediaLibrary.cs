using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public class MediaLibrary : IMediaLibrary
    {
        public MediaLibrary()
        {
        }

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
            //providers.Add(new PlaylistProvider());
            return providers;
        }

        public IEnumerable<IPlaylistProvider> PlaylistProviders => Providers.OfType<IPlaylistProvider>();
        public IEnumerable<IArtistProvider> ArtistProviders => Providers.OfType<IArtistProvider>();
        public IEnumerable<IAlbumProvider> AlbumProviders => Providers.OfType<IAlbumProvider>();
        public IEnumerable<IMediaItemProvider> MediaItemProviders => Providers.OfType<IMediaItemProvider>();

        public Task AddOrUpdateAlbum(IAlbum playlist)
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdateArtist(IArtist artist)
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdateMediaItem(IMediaItem mediaItem)
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdatePlaylist(IPlaylist playlist)
        {
            throw new NotImplementedException();
        }

        public Task<IAlbum> GetAlbum(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAlbum>> GetAlbums()
        {
            throw new NotImplementedException();
        }

        public Task<IArtist> GetArtist(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IArtist>> GetArtists()
        {
            throw new NotImplementedException();
        }

        public Task<IMediaItem> GetMediaItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IMediaItem>> GetMediaÍtems(string search)
        {
            var mediaItems = new List<IMediaItem>();

            IList<Task<IEnumerable<IMediaItem>>> tasks = new List<Task<IEnumerable<IMediaItem>>>();
            foreach (var provider in MediaItemProviders)
            {
                tasks.Add(provider.GetMediaÍtems(search));
            }
            foreach (var item in await Task.WhenAll(tasks))
            {
                mediaItems.AddRange(item);
            }
            return mediaItems;
        }

        public async Task<IPlaylist> GetPlaylist(Guid id)
        {
            foreach (var provider in PlaylistProviders)
            {
                var item = await provider.GetPlaylist(id);
                if (item != null)
                    return item;
            }
            return null;
        }

        public async Task<IEnumerable<IPlaylist>> GetPlaylists()
        {
            foreach (var provider in PlaylistProviders)
            {
                var item = await provider.GetPlaylists();
                if (item != null)
                    return item;
            }
            return null;
        }
    }
}

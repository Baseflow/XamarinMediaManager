using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Media;

namespace MediaManager.Media
{
    public interface IMediaLibrary : IPlaylistProvider, IArtistProvider, IAlbumProvider, IMediaItemProvider
    {
        IList<ILibraryProvider> Providers { get; }

        IEnumerable<IPlaylistProvider> PlaylistProviders { get; }
        IEnumerable<IArtistProvider> ArtistProviders { get;}
        IEnumerable<IAlbumProvider> AlbumProviders { get; }
        IEnumerable<IMediaItemProvider> MediaItemProviders { get; }
    }
}

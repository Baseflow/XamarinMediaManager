using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IAlbumProvider : ILibraryProvider
    {
        Task<IEnumerable<IAlbum>> GetAlbums();
        Task<IAlbum> GetAlbum(Guid id);
        Task AddOrUpdateAlbum(IAlbum playlist);
    }
}

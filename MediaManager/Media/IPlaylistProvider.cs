using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IPlaylistProvider : ILibraryProvider
    {
        Task<IEnumerable<IPlaylist>> GetPlaylists();
        Task<IPlaylist> GetPlaylist(Guid id);
        Task AddOrUpdatePlaylist(IPlaylist playlist);
    }
}

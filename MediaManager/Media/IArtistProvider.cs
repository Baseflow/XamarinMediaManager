using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Library;

namespace MediaManager.Media
{
    public interface IArtistProvider : ILibraryProvider
    {
        Task<IEnumerable<IArtist>> GetArtists();
        Task<IArtist> GetArtist(Guid id);
        Task AddOrUpdateArtist(IArtist artist);
    }
}

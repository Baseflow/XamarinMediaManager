using System.Collections.Generic;

namespace MediaManager.Media
{
    public interface IPlaylist : IList<IMediaItem>
    {
        string Title { get; set; }
    }
}

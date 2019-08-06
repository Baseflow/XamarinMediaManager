using System;
using System.Collections.Generic;
using MediaManager.Media;

namespace MediaManager.Library
{
    public interface IAlbum : IList<IMediaItem>
    {
        string AlbumId { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        string Tags { get; set; }

        string Genre { get; set; }

        object Image { get; set; }

        string ImageUri { get; set; }

        object Rating { get; set; }

        DateTime ReleaseDate { get; set; }

        TimeSpan Duration { get; set; }

        string LabelName { get; set; }

        IList<IArtist> Artists { get; set; }
    }
}

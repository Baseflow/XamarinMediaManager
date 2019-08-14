using System;
using System.Collections.Generic;

namespace MediaManager.Library
{
    public class Artist : IArtist
    {
        public Artist()
        {
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Tags { get; set; }
        public string Genre { get; set; }
        public object Image { get; set; }
        public string ImageUri { get; set; }
        public object Rating { get; set; }
        public IList<IAlbum> Albums { get; set; }
        public IList<IMediaItem> TopTracks { get; set; }
    }
}

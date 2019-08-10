using System;
using System.Collections.Generic;

namespace MediaManager.Library
{
    public class Album : List<IMediaItem>, IAlbum
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Genre { get; set; }
        public object Image { get; set; }
        public string ImageUri { get; set; }
        public string LabelName { get; set; }
        public object Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public TimeSpan Duration { get; set; }
        public IList<IArtist> Artists { get; set; }
    }
}

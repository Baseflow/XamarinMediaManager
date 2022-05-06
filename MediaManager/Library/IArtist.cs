namespace MediaManager.Library
{
    public interface IArtist : IContentItem
    {
        string Name { get; set; }

        string Biography { get; set; }

        string Tags { get; set; }

        string Genre { get; set; }

        object Image { get; set; }

        string ImageUri { get; set; }

        object Rating { get; set; }

        IList<IAlbum> Albums { get; set; }

        IList<IMediaItem> TopTracks { get; set; }

        IList<IMediaItem> AllTracks { get; }
    }
}

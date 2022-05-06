namespace MediaManager.Library
{
    public interface IAlbum : IContentItem
    {
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

        IList<IMediaItem> MediaItems { get; set; }
    }
}

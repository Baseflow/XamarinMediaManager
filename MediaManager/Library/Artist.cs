namespace MediaManager.Library
{
    public class Artist : ContentItem, IArtist
    {
        public Artist()
        {
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _biography;
        public string Biography
        {
            get => _biography;
            set => SetProperty(ref _biography, value);
        }

        private string _tags;
        public string Tags
        {
            get => _tags;
            set => SetProperty(ref _tags, value);
        }

        private string _genre;
        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        private object _image;
        public object Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        private string _imageUri;
        public string ImageUri
        {
            get => _imageUri;
            set => SetProperty(ref _imageUri, value);
        }

        private object _rating;
        public object Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private IList<IAlbum> _albums = new List<IAlbum>();
        public IList<IAlbum> Albums
        {
            get => _albums;
            set => SetProperty(ref _albums, value);
        }

        private IList<IMediaItem> _topTracks = new List<IMediaItem>();
        public IList<IMediaItem> TopTracks
        {
            get => _topTracks;
            set => SetProperty(ref _topTracks, value);
        }

        public virtual IList<IMediaItem> AllTracks => Albums?.SelectMany(x => x.MediaItems)?.ToList();
    }
}

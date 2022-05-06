namespace MediaManager.Library
{
    public class Album : ContentItem, IAlbum
    {
        public Album()
        {
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
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

        private string _labelName;
        public string LabelName
        {
            get => _labelName;
            set => SetProperty(ref _labelName, value);
        }

        private object _rating;
        public object Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private DateTime _releaseDate;
        public DateTime ReleaseDate
        {
            get => _releaseDate;
            set => SetProperty(ref _releaseDate, value);
        }

        private TimeSpan _duration;
        public virtual TimeSpan Duration
        {
            get
            {
                if (_duration == null)
                {
                    //Return the total of all media items when no value is set
                    var totalDuration = new TimeSpan();
                    MediaItems?.Select(x => totalDuration.Add(x.Duration));
                    return totalDuration;
                }
                return _duration;
            }
            set => SetProperty(ref _duration, value);
        }

        private IList<IArtist> _artists = new List<IArtist>();
        public IList<IArtist> Artists
        {
            get => _artists;
            set => SetProperty(ref _artists, value);
        }

        private IList<IMediaItem> _mediaItems = new List<IMediaItem>();
        public IList<IMediaItem> MediaItems
        {
            get => _mediaItems;
            set => SetProperty(ref _mediaItems, value);
        }
    }
}

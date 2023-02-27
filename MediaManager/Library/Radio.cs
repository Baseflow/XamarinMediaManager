namespace MediaManager.Library
{
    public class Radio : ContentItem, IRadio
    {
        public Radio()
        {
        }

        private string _uri;
        public string Uri
        {
            get => _uri;
            set => SetProperty(ref _uri, value);
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

        private object _rating;
        public object Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private DateTime _createdAt = DateTime.Now;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        private DateTime _updatedAt;
        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set => SetProperty(ref _updatedAt, value);
        }

        private SharingType _sharingType = SharingType.Public;
        public SharingType SharingType
        {
            get => _sharingType;
            set => SetProperty(ref _sharingType, value);
        }

        private IList<IMediaItem> _mediaItems = new List<IMediaItem>();
        public IList<IMediaItem> MediaItems
        {
            get => _mediaItems;
            set => SetProperty(ref _mediaItems, value);
        }
    }
}

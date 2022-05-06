namespace MediaManager.Library
{
    public class ContentItem : NotifyPropertyChangedBase, IContentItem
    {
        public ContentItem()
        {
        }

        private string _id = Guid.NewGuid().ToString();

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
    }
}

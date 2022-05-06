using MediaManager.Library;

namespace MediaManager.Media
{
    public abstract class LibraryProvider<TContentItem> : ILibraryProvider<TContentItem> where TContentItem : IContentItem
    {
        public bool Enabled { get; set; } = true;

        public abstract Task<bool> AddOrUpdate(TContentItem item);
        public abstract Task<bool> Exists(string id);
        public abstract Task<TContentItem> Get(string id);
        public abstract Task<IEnumerable<TContentItem>> GetAll();
        public abstract Task<bool> RemoveAll();
        public abstract Task<bool> Remove(TContentItem item);
    }
}

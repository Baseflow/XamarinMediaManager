using MediaManager.Library;

namespace MediaManager.Media
{
    public interface ILibraryProvider
    {
        bool Enabled { get; set; }
    }

    public interface ILibraryProvider<TContentItem> : ILibraryProvider where TContentItem : IContentItem
    {
        Task<IEnumerable<TContentItem>> GetAll();
        Task<TContentItem> Get(string id);
        Task<bool> Exists(string id);
        Task<bool> AddOrUpdate(TContentItem item);
        Task<bool> Remove(TContentItem item);
        Task<bool> RemoveAll();
    }
}

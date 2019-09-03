using System.ComponentModel;

namespace MediaManager.Library
{
    public interface IContentItem : INotifyPropertyChanged
    {
        string Id { get; set; }
    }
}

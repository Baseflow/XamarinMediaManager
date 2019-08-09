using System;
using System.Collections.ObjectModel;

namespace MediaManager.Library
{
    public class Radio : ObservableCollection<IMediaItem>, IRadio
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Uri { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Genre { get; set; }
        public object Image { get; set; }
        public string ImageUri { get; set; }
        public object Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public SharingType SharingType { get; set; }
    }
}

using System;
using System.ComponentModel;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Tests
{
    class MediaFile : IMediaFile
    {
        public Guid Id { get; set; } = new Guid();
        public MediaFileType Type { get; set; }
        public string Url { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format("[MediaFile: Id={0}, Type={1}, Url={2}]", Id, Type, Url);
        }

        public object Cover
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

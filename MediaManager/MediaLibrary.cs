using System;
using MediaManager.Media;

namespace MediaManager
{
    public class MediaLibrary : IMediaLibrary
    {
        public IMediaList Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

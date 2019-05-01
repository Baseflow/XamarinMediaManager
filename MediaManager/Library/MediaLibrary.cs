using System;
using MediaManager.Media;

namespace MediaManager.Library
{
    public class MediaLibrary : IMediaLibrary
    {
        public IMediaList Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

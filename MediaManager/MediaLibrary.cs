using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Media;
using MediaManager.Queue;

namespace MediaManager
{
    public class MediaLibrary : IMediaLibrary
    {
        public IMediaList Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

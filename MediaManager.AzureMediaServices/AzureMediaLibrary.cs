using System;
using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.AzureMediaServices
{
    public class AzureMediaLibrary : IMediaLibrary
    {
        public IMediaList Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

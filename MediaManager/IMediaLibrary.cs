using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Media;

namespace MediaManager
{
    public interface IMediaLibrary
    {
        IMediaList Items { get; set; }
    }
}

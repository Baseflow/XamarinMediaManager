using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Playback
{
    public class VideoSizeChangedEventArgs : EventArgs
    {
        public VideoSizeChangedEventArgs(VideoSize size)
        {
            Size = size;
        }

        public VideoSize Size { get; }
    }
}

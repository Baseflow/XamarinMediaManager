using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Playback
{
    public class VideoSizeChangedEventArgs : EventArgs
    {
        public VideoSizeChangedEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
    }
}

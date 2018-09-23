using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.Media
{
    public class MediaPlayerEventArgs : EventArgs
    {
        public MediaPlayerEventArgs(IMediaItem mediaItem, IMediaPlayer mediaPlayer)
        {
            MediaItem = mediaItem;
            MediaPlayer = mediaPlayer;
        }

        public IMediaItem MediaItem { get; private set; }
        public IMediaPlayer MediaPlayer { get; private set; }
    }
}

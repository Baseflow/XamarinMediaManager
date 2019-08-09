using System;
using MediaManager.Library;

namespace MediaManager.Player
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

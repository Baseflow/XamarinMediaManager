using MediaManager.Library;
using MediaManager.Media;

namespace MediaManager.Player
{
    public class MediaPlayerEventArgs : MediaItemEventArgs
    {
        public MediaPlayerEventArgs(IMediaItem mediaItem, IMediaPlayer mediaPlayer) : base(mediaItem)
        {
            MediaPlayer = mediaPlayer;
        }

        public IMediaPlayer MediaPlayer { get; protected set; }
    }
}

using Plugin.MediaManager.MediaSession;

namespace Plugin.MediaManager.Audio
{
    public class AudioPlayerImplementation : AudioPlayerBase<MediaPlayerService>
    {
        public AudioPlayerImplementation(MediaSessionManagerImplementation sessionManager) : base(sessionManager)
        {
        }
    }
}

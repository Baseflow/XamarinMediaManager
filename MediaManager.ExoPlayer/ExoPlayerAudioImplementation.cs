using Plugin.MediaManager.MediaSession;

namespace Plugin.MediaManager.ExoPlayer
{
    public class ExoPlayerAudioImplementation : AudioPlayerBase<ExoPlayerAudioService>
    {
        public ExoPlayerAudioImplementation(MediaSessionManager sessionManager) : base(sessionManager)
        {
        }
    }
}

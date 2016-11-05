using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Audio;
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

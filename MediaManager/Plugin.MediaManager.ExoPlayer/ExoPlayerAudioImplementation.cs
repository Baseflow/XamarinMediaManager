using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager.ExoPlayer
{
    public class ExoPlayerAudioImplementation : AudioPlayerImplementation<ExoPlayerAudioService>
    {
        public ExoPlayerAudioImplementation(MediaSessionManagerImplementation sessionManager) : base(sessionManager)
        {
        }
    }
}

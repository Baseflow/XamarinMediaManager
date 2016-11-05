using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Audio;
using Plugin.MediaManager.MediaSession;

namespace Plugin.MediaManager.ExoPlayer
{
    public class ExoPlayerAudioImplementation : AudioPlayerImplementation
    {
        public ExoPlayerAudioImplementation(MediaSessionManagerImplementation sessionManager) : base(sessionManager)
        {
        }

        public override Android.Content.Intent GetMediaServiceIntent()
        {
            return new Intent(applicationContext, typeof(ExoPlayerAudioService));
        }
    }
}

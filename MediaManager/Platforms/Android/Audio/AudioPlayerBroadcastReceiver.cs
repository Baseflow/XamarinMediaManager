using Android.Content;
using Android.Media;
using Plugin.MediaManager.MediaSession;

namespace Plugin.MediaManager.Audio
{
    /// <summary>
    /// This is a simple intent receiver that is used to stop playback
    /// when audio become noisy, such as the user unplugged headphones
    /// </summary>

    public class AudioPlayerBroadcastReceiver : BroadcastReceiver
    {
        private readonly MediaSessionManager _manager;

        public AudioPlayerBroadcastReceiver(MediaSessionManager manager)
        {
            _manager = manager;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            if (AudioManager.ActionAudioBecomingNoisy.Equals(intent.Action))
            {
                _manager.HandleAction(MediaServiceBase.ActionPause);
            }
        }
    }
}
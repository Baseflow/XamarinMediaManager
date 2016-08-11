using Android.App;
using Android.Content;
using Android.Media;

namespace Plugin.MediaManager
{
    /// <summary>
    /// This is a simple intent receiver that is used to stop playback
    /// when audio become noisy, such as the user unplugged headphones
    /// </summary>
    [BroadcastReceiver]
	[Android.App.IntentFilter(new []{AudioManager.ActionAudioBecomingNoisy})]
    public class MusicBroadcastReceiver: BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != AudioManager.ActionAudioBecomingNoisy)
                return;

            //signal the service to stop!
            var stopIntent = new Intent(MediaPlayerService.ActionStop);
            context.StartService(stopIntent);
        }
    }
}
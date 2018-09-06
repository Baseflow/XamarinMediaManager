using Android.Content;
using Android.Media;

namespace MediaManager.Platforms.Android.Audio
{
    class BecomingNoisyReceiver : BroadcastReceiver
    {
        private readonly IntentFilter NoisyIntentFilter = new IntentFilter(AudioManager.ActionAudioBecomingNoisy);
        private Context context;
        private AudioFocusManager audioFocusManager;

        public BecomingNoisyReceiver(Context context, AudioFocusManager audioFocusManager)
        {
            this.context = context;
            this.audioFocusManager = audioFocusManager;
        }

        private bool registered = false;

        public void Register()
        {
            if (!registered)
            {
                context.RegisterReceiver(this, NoisyIntentFilter);
                registered = true;
            }
        }

        public void Unregister()
        {
            if (registered)
            {
                context.UnregisterReceiver(this);
                registered = false;
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == AudioManager.ActionAudioBecomingNoisy)
            {
                audioFocusManager.AbandonAudioFocus();
            }
        }
    }
}

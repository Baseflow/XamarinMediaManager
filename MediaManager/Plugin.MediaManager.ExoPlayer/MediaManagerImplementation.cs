using Android.App;
using Android.Content.Res;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.ExoPlayer;

namespace Plugin.MediaManager.ExoPlayer
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : Plugin.MediaManager.MediaManagerImplementation
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videPlayer;

        public override IAudioPlayer AudioPlayer
        {
            get {return _audioPlayer ?? (_audioPlayer = new AudioPlayerImplementation<ExoPlayerAudioService>(MediaSessionManager));}
            set { _audioPlayer = value; }
        }

        public override IVideoPlayer VideoPlayer
        {
            get { return _videPlayer ?? (_videPlayer = new ExoPlayerVideoImplementation()); }
            set { _videPlayer = value; }
        }

        public override IMediaNotificationManager MediaNotificationManager
        {
            get { return MediaSessionManager.NotificationManager; }
            set { MediaSessionManager.NotificationManager = value; }
        }
    }
}
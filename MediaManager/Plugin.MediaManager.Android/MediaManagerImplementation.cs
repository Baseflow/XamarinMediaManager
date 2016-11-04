using Android.App;
using Android.Content.Res;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.ExoPlayer;

namespace Plugin.MediaManager
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : MediaManagerBase
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videPlayer;
        private IMediaExtractor _mediaExtraxtor;

        public override IAudioPlayer AudioPlayer
        {
            get {return _audioPlayer ?? (_audioPlayer = new AudioPlayerImplementation<ExoPlayerAudioService>(MediaSessionManager));}
            set { _audioPlayer = value; }
        }

        public override IVideoPlayer VideoPlayer
        {
            get { return _videPlayer ?? (_videPlayer = new VideoPlayerImplementation()); }
            set { _videPlayer = value; }
        }

        public override IMediaNotificationManager MediaNotificationManager
        {
            get { return MediaSessionManager.NotificationManager; }
            set { MediaSessionManager.NotificationManager = value; }
        }

        public override IMediaExtractor MediaExtractor
        {
            get { return _mediaExtraxtor ?? (_mediaExtraxtor = new MediaExtractorImplementation(Resources.System, RequestProperties)); }
            set { _mediaExtraxtor = value; }
        }

        public MediaSessionManagerImplementation MediaSessionManager { get; set; } = new MediaSessionManagerImplementation(Application.Context);

        public MediaManagerImplementation()
        {
            MediaSessionManager.OnNotificationActionFired += HandleNotificationActions;
        }

        private async void HandleNotificationActions(object sender, string action)
        {
            if (action.Equals(MediaServiceBase.ActionPlay) || action.Equals(MediaServiceBase.ActionPause))
            {
                await PlayPause();
            }
            else if (action.Equals(MediaServiceBase.ActionPrevious))
            {
                await PlayPrevious();
            }
            else if (action.Equals(MediaServiceBase.ActionNext))
            {
                await PlayNext();
            }
            else if (action.Equals(MediaServiceBase.ActionStop))
            {
                await Stop();
            }
        }
    }
}
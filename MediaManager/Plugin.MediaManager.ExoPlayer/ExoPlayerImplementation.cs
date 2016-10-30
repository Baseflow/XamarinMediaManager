using System;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager.ExoPlayer
{
    public class ExoPlayerImplementation : MediaManagerImplementation
    {
        private IAudioPlayer _audioPlayer;
        private IVideoPlayer _videPlayer;

        public override IAudioPlayer AudioPlayer
        {
            get { return _audioPlayer ?? (_audioPlayer = new ExoPlayerAudioImplementation(MediaSessionManager)); }
            set { _audioPlayer = value; }
        }

        public override IVideoPlayer VideoPlayer
        {
            get { return _videPlayer ?? (_videPlayer = new ExoPlayerVideoImplementation()); }
            set { _videPlayer = value; }
        }
    }
}

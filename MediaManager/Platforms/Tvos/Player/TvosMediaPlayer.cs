
using System;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Tvos.Video;
using MediaManager.Player;
using MediaManager.Video;

namespace MediaManager.Platforms.Tvos.Media
{
    public class TvosMediaPlayer : AppleMediaPlayer, IMediaPlayer<AVQueuePlayer, VideoView>
    {
        public VideoView PlayerView => VideoView as VideoView;

        private IVideoView _videoView;
        public override IVideoView VideoView
        {
            get => _videoView;
            set
            {
                _videoView = value;
                if (PlayerView != null)
                {
                    PlayerView.PlayerViewController.Player = Player;
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            var audioSession = AVAudioSession.SharedInstance();
            try
            {
                audioSession.SetCategory(AVAudioSession.CategoryPlayback);
                NSError activationError = null;
                audioSession.SetActive(true, out activationError);
                if (activationError != null)
                    Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetActive(false);
            base.Dispose(disposing);
        }
    }
}

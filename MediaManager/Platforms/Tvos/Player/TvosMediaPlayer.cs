
using System;
using AVFoundation;
using Foundation;
using MediaManager.Platforms.Apple.Player;
using MediaManager.Platforms.Tvos.Video;
using MediaManager.Player;
using MediaManager.Video;

namespace MediaManager.Platforms.Tvos.Player
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

        public override void UpdateVideoAspect(VideoAspectMode videoAspectMode)
        {
            if (PlayerView == null)
                return;

            var playerViewController = PlayerView.PlayerViewController;

            switch (videoAspectMode)
            {
                case VideoAspectMode.None:
                    playerViewController.VideoGravity = AVLayerVideoGravity.Resize;
                    break;
                case VideoAspectMode.AspectFit:
                    playerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                    break;
                case VideoAspectMode.AspectFill:
                    playerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                    break;
                default:
                    playerViewController.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                    break;
            }
        }

        public override void UpdateShowPlaybackControls(bool showPlaybackControls)
        {
            if (PlayerView == null)
                return;

            PlayerView.PlayerViewController.ShowsPlaybackControls = showPlaybackControls;
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

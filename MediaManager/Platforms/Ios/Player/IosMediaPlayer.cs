using System;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Ios.Video;
using MediaManager.Player;
using MediaManager.Video;
using MediaPlayer;
using UIKit;

namespace MediaManager.Platforms.Ios.Media
{
    public class IosMediaPlayer : AppleMediaPlayer, IMediaPlayer<AVQueuePlayer, VideoView>
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
                audioSession.SetActive(true, out NSError activationError);
                if (activationError != null)
                    Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
            }
            catch
            {
            }

            InvokeOnMainThread(() => {
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });
        }

        protected override void Dispose(bool disposing)
        {
            InvokeOnMainThread(() =>
            {
                UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
            });

            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetActive(false);
            base.Dispose(disposing);
        }
    }
}

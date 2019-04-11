using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Ios.Video;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Media
{
    public class MediaPlayer : AppleMediaPlayer, IVideoPlayer<AVPlayer, VideoSurface>
    {
        //TODO: Make possible to hook into
        AVPlayerViewController aVPlayerViewController;

        public VideoSurface PlayerView { get; set; }

        public override void Initialize()
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

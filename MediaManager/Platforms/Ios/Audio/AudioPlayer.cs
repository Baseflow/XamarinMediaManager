using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Platforms.Apple.Audio;

namespace MediaManager.Platforms.Ios.Audio
{
    public class AudioPlayer : AppleAudioPlayer
    {
        public AudioPlayer()
        {
        }

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
    }
}

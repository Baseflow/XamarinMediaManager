﻿using System;
using AVFoundation;
using MediaManager.Platforms.Apple.Player;
using MediaManager.Platforms.Ios.Video;
using MediaManager.Player;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Player
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
                SetProperty(ref _videoView, value);
                if (PlayerView != null)
                {
                    PlayerView.PlayerViewController.Player = Player;
                    UpdateVideoView();
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
                audioSession.SetActive(true, out var activationError);
                if (activationError != null)
                    Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);

                AVAudioSession.Notifications.ObserveInterruption(ToneInterruptionListener);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Player.InvokeOnMainThread(() =>
            {
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });
        }

        private async void ToneInterruptionListener(object sender, AVAudioSessionInterruptionEventArgs interruptArgs)
        {
            switch (interruptArgs.InterruptionType)
            {
                case AVAudioSessionInterruptionType.Began:
                    await MediaManager.Pause();
                    break;
                case AVAudioSessionInterruptionType.Ended:
                    if(interruptArgs.Option == AVAudioSessionInterruptionOptions.ShouldResume)
                    {
                        await MediaManager.Play();
                    }
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Player.InvokeOnMainThread(() =>
            {
                UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
            });

            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetActive(false);

            base.Dispose(disposing);
        }
    }
}

using System;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Platforms.Apple.Media;
using MediaManager.Platforms.Ios.Video;
using MediaManager.Video;
using MediaPlayer;
using UIKit;

namespace MediaManager.Platforms.Ios.Media
{
    public class MediaPlayer : AppleMediaPlayer, IMediaPlayer<AVPlayer, VideoView>
    {
        public VideoView PlayerView { get; set; }
        public override IVideoView VideoView => PlayerView;

        protected override void Init()
        {
            base.Init();
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

            var commandCenter = MPRemoteCommandCenter.Shared;

            commandCenter.TogglePlayPauseCommand.Enabled = true;
            commandCenter.TogglePlayPauseCommand.AddTarget(PlayPauseCommand);

            commandCenter.PlayCommand.Enabled = true;
            commandCenter.PlayCommand.AddTarget(PlayCommand);

            commandCenter.ChangeRepeatModeCommand.Enabled = true;
            commandCenter.ChangeRepeatModeCommand.AddTarget(RepeatCommand);

            commandCenter.ChangeShuffleModeCommand.Enabled = true;
            commandCenter.ChangeShuffleModeCommand.AddTarget(ShuffleCommand);

            commandCenter.NextTrackCommand.Enabled = true;
            commandCenter.NextTrackCommand.AddTarget(NextCommand);

            commandCenter.PauseCommand.Enabled = true;
            commandCenter.PauseCommand.AddTarget(PauseCommand);

            commandCenter.PreviousTrackCommand.Enabled = true;
            commandCenter.PreviousTrackCommand.AddTarget(PreviousCommand);

            commandCenter.SeekBackwardCommand.Enabled = true;
            commandCenter.SeekBackwardCommand.AddTarget(SeekBackwardCommand);

            commandCenter.SeekForwardCommand.Enabled = true;
            commandCenter.SeekForwardCommand.AddTarget(SeekForwardCommand);

            commandCenter.SkipBackwardCommand.Enabled = true;
            commandCenter.SkipBackwardCommand.AddTarget(SkipBackwardCommand);

            commandCenter.SkipForwardCommand.Enabled = true;
            commandCenter.SkipForwardCommand.AddTarget(SkipForwardCommand);

            commandCenter.StopCommand.Enabled = true;
            commandCenter.StopCommand.AddTarget(StopCommand);
        }

        private MPRemoteCommandHandlerStatus SkipBackwardCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.StepBackward();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus SkipForwardCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.StepForward();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus StopCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.Stop();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus SeekForwardCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.StepForward();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus SeekBackwardCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.StepBackward();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus PreviousCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.PlayPrevious();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus PauseCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.Pause();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus NextCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.PlayNext();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus ShuffleCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.ToggleShuffle();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus RepeatCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.ToggleRepeat();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus PlayCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.Play();
            return MPRemoteCommandHandlerStatus.Success;
        }

        private MPRemoteCommandHandlerStatus PlayPauseCommand(MPRemoteCommandEvent arg)
        {
            MediaManager.PlayPause();
            return MPRemoteCommandHandlerStatus.Success;
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

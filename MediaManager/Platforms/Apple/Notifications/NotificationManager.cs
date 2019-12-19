using System;
using Foundation;
using MediaManager.Notifications;
using MediaPlayer;
using UIKit;

namespace MediaManager.Platforms.Apple.Notifications
{
    public class NotificationManager : NotificationManagerBase
    {
        public NotificationManager()
        {
            Enabled = true;
        }

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Apple;
        protected MPRemoteCommandCenter CommandCenter = MPRemoteCommandCenter.Shared;

        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                ShowPlayPauseControls = value;
                ShowNavigationControls = value;
            }
        }

        public override bool ShowPlayPauseControls
        {
            get => base.ShowPlayPauseControls;
            set
            {
                base.ShowPlayPauseControls = value;
                if (ShowPlayPauseControls)
                {
                    CommandCenter.TogglePlayPauseCommand.Enabled = true;
                    CommandCenter.TogglePlayPauseCommand.AddTarget(PlayPauseCommand);

                    CommandCenter.PlayCommand.Enabled = true;
                    CommandCenter.PlayCommand.AddTarget(PlayCommand);

                    CommandCenter.PauseCommand.Enabled = true;
                    CommandCenter.PauseCommand.AddTarget(PauseCommand);

                    CommandCenter.StopCommand.Enabled = true;
                    CommandCenter.StopCommand.AddTarget(StopCommand);
                }
                else
                {
                    CommandCenter.TogglePlayPauseCommand.Enabled = false;

                    CommandCenter.PlayCommand.Enabled = false;

                    CommandCenter.PauseCommand.Enabled = false;

                    CommandCenter.StopCommand.Enabled = false;
                }
            }
        }

        public override bool ShowNavigationControls
        {
            get => base.ShowNavigationControls;
            set
            {
                base.ShowNavigationControls = value;
                if (ShowNavigationControls)
                {
                    CommandCenter.NextTrackCommand.Enabled = true;
                    CommandCenter.NextTrackCommand.AddTarget(NextCommand);

                    CommandCenter.PreviousTrackCommand.Enabled = true;
                    CommandCenter.PreviousTrackCommand.AddTarget(PreviousCommand);

                    CommandCenter.SeekBackwardCommand.Enabled = true;
                    CommandCenter.SeekBackwardCommand.AddTarget(SeekBackwardCommand);

                    CommandCenter.SeekForwardCommand.Enabled = true;
                    CommandCenter.SeekForwardCommand.AddTarget(SeekForwardCommand);

                    CommandCenter.SkipBackwardCommand.Enabled = true;
                    CommandCenter.SkipBackwardCommand.AddTarget(SkipBackwardCommand);

                    CommandCenter.SkipForwardCommand.Enabled = true;
                    CommandCenter.SkipForwardCommand.AddTarget(SkipForwardCommand);

                    CommandCenter.ChangeRepeatModeCommand.Enabled = true;
                    CommandCenter.ChangeRepeatModeCommand.AddTarget(RepeatCommand);

                    CommandCenter.ChangeShuffleModeCommand.Enabled = true;
                    CommandCenter.ChangeShuffleModeCommand.AddTarget(ShuffleCommand);
                }
                else
                {
                    CommandCenter.NextTrackCommand.Enabled = false;

                    CommandCenter.PreviousTrackCommand.Enabled = false;

                    CommandCenter.SeekBackwardCommand.Enabled = false;

                    CommandCenter.SeekForwardCommand.Enabled = false;

                    CommandCenter.SkipBackwardCommand.Enabled = false;

                    CommandCenter.SkipForwardCommand.Enabled = false;

                    CommandCenter.ChangeRepeatModeCommand.Enabled = false;

                    CommandCenter.ChangeShuffleModeCommand.Enabled = false;
                }
            }
        }

        public override void UpdateNotification()
        {
            var mediaItem = MediaManager.Queue.Current;

            if (mediaItem == null || !Enabled)
            {
                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = null;
                return;
            }

            var nowPlayingInfo = new MPNowPlayingInfo
            {
                Title = mediaItem.Title,
                AlbumTitle = mediaItem.Album,
                AlbumTrackNumber = mediaItem.TrackNumber,
                AlbumTrackCount = mediaItem.NumTracks,
                Artist = mediaItem.Artist,
                Composer = mediaItem.Composer,
                DiscNumber = mediaItem.DiscNumber,
                Genre = mediaItem.Genre,
                ElapsedPlaybackTime = MediaManager.Position.TotalSeconds,
                PlaybackDuration = MediaManager.Duration.TotalSeconds,
                PlaybackQueueIndex = MediaManager.Queue.CurrentIndex,
                PlaybackQueueCount = MediaManager.Queue.Count
            };

            if (MediaManager.IsPlaying())
            {
                nowPlayingInfo.PlaybackRate = 1f;
            }
            else
            {
                nowPlayingInfo.PlaybackRate = 0f;
            }

#if __IOS__ || __TVOS__
            object image = null;
            try
            {
                if (!string.IsNullOrEmpty(mediaItem.ImageUri))
                {
                    if (mediaItem.ImageUri.StartsWith("http", StringComparison.CurrentCulture))
                    {
                        image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(mediaItem.ImageUri)));
                    }
                    else
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(9,0))
                        {
                            image = UIImage.FromBundle(mediaItem.ImageUri);
                        }
                    }
                }
                if (image == null && !string.IsNullOrEmpty(mediaItem.AlbumImageUri))
                {
                    if (mediaItem.AlbumImageUri.StartsWith("http", StringComparison.CurrentCulture))
                    {
                        image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(mediaItem.AlbumImageUri)));
                    }
                    else
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                        {
                            image = UIImage.FromBundle(mediaItem.AlbumImageUri);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (image != null)
            {
                //TODO: Why is this deprecated?
                nowPlayingInfo.Artwork = new MPMediaItemArtwork((UIKit.UIImage)image);
            }
#endif
            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = nowPlayingInfo;
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
    }
}

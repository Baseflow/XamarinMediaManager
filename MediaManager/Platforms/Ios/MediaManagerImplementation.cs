using MediaPlayer;
using UIKit;

namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Ios.Media.MediaPlayer>
    {
        public MediaManagerImplementation()
        {
            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();

            var commandCenter = MPRemoteCommandCenter.Shared;
            commandCenter.TogglePlayPauseCommand.Enabled = true;
            commandCenter.TogglePlayPauseCommand.AddTarget(TogglePlayPauseCommand);
        }

        private MPRemoteCommandHandlerStatus TogglePlayPauseCommand(MPRemoteCommandEvent arg)
        {
            this.PlayPause();
            return MPRemoteCommandHandlerStatus.Success;
        }
    }
}

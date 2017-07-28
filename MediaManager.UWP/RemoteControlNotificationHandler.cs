using Windows.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.SystemWrappers;

namespace Plugin.MediaManager
{
    public class RemoteControlNotificationHandler
    {
        private readonly ISystemMediaTransportControlsWrapper _systemMediaTransportControlsWrapper;
        private readonly IPlaybackController _playbackController;

        public RemoteControlNotificationHandler(ISystemMediaTransportControlsWrapper systemMediaTransportControlsWrapper, IPlaybackController playbackController)
        {
            _systemMediaTransportControlsWrapper = systemMediaTransportControlsWrapper;
            _playbackController = playbackController;

            _systemMediaTransportControlsWrapper.IsNextEnabled = true;
            _systemMediaTransportControlsWrapper.IsPreviousEnabled = true;
            _systemMediaTransportControlsWrapper.IsPlayEnabled = true;
        }

        public void SubscribeToNotifications()
        {
            _systemMediaTransportControlsWrapper.SubscribeToMediaButtonEvents();
            _systemMediaTransportControlsWrapper.ButtonPressed += MediaControls_ButtonPressed;
        }

        public void UnsubscribeFromNotifications()
        {
            _systemMediaTransportControlsWrapper.UnsubscribeFromMediaButtonEvents();
            _systemMediaTransportControlsWrapper.ButtonPressed -= MediaControls_ButtonPressed;
        }

        private async void MediaControls_ButtonPressed(SystemMediaTransportControls sender, ISystemMediaTransportControlsButtonPressedEventArgsWrapper args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Next:
                    await _playbackController.PlayNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await _playbackController.PlayPrevious();
                    break;
                case SystemMediaTransportControlsButton.Play:
                    await _playbackController.PlayPause();
                    break;
            }
        }
    }
}

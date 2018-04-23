using Windows.Media;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.SystemWrappers;

namespace Plugin.MediaManager
{
    internal class MediaButtonPlaybackController
    {
        private readonly ISystemMediaTransportControlsWrapper _systemMediaTransportControlsWrapper;
        private readonly IPlaybackControllerProvider _playbackControllerProvider;

        public MediaButtonPlaybackController(ISystemMediaTransportControlsWrapper systemMediaTransportControlsWrapper, IPlaybackControllerProvider playbackControllerProvider)
        {
            _systemMediaTransportControlsWrapper = systemMediaTransportControlsWrapper;
            _playbackControllerProvider = playbackControllerProvider;

            _systemMediaTransportControlsWrapper.IsNextEnabled = true;
            _systemMediaTransportControlsWrapper.IsPreviousEnabled = true;
            _systemMediaTransportControlsWrapper.IsPlayEnabled = true;
            _systemMediaTransportControlsWrapper.IsPauseEnabled = true;
            _systemMediaTransportControlsWrapper.IsStopEnabled = true;
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
                    await _playbackControllerProvider.PlaybackController.PlayNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await _playbackControllerProvider.PlaybackController.PlayPreviousOrSeekToStart();
                    break;
                case SystemMediaTransportControlsButton.Play:
                    await _playbackControllerProvider.PlaybackController.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await _playbackControllerProvider.PlaybackController.Pause();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    await _playbackControllerProvider.PlaybackController.Stop();
                    break;
            }
        }
    }
}

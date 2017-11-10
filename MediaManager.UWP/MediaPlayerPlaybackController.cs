using Windows.Media;
using Windows.Media.Playback;
using Plugin.MediaManager.Interfaces;

namespace Plugin.MediaManager
{
    internal class MediaPlayerPlaybackController : IMediaPlyerPlaybackController
    {
        private readonly IPlaybackControllerProvider _playbackControllerProvider;

        public MediaPlayerPlaybackController(IPlaybackControllerProvider playbackControllerProvider)
        {
            _playbackControllerProvider = playbackControllerProvider;

            Player.SystemMediaTransportControls.IsNextEnabled = true;
            Player.SystemMediaTransportControls.IsPreviousEnabled = true;
            Player.SystemMediaTransportControls.IsPlayEnabled = true;
            Player.SystemMediaTransportControls.IsPauseEnabled = true;
            Player.SystemMediaTransportControls.IsStopEnabled = true;

            Player.SystemMediaTransportControls.ButtonPressed += ButtonPressed;
        }

        public MediaPlayer Player { get; } = new MediaPlayer();

        public void Dispose()
        {
            Player.SystemMediaTransportControls.ButtonPressed -= ButtonPressed;
        }

        private async void ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
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

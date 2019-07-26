using MediaManager.Player;

namespace MediaManager.Forms.Xaml
{
    public class PlayExtension : MediaExtensionBase
    {
        public PlayExtension()
            : base()
        {
        }

        protected override bool CanExecute()
        {
            return MediaManager.State == MediaPlayerState.Paused ||
                MediaManager.State == MediaPlayerState.Stopped;
        }

        protected override void Execute() =>
            MediaManager.Play();
    }
}

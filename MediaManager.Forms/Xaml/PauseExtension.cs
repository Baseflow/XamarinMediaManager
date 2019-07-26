using MediaManager.Player;

namespace MediaManager.Forms.Xaml
{
    public class PauseExtension : MediaExtensionBase
    {
        public PauseExtension()
            : base()
        {
        }

        protected override bool CanExecute() =>
            MediaManager.State == MediaPlayerState.Playing;

        protected override void Execute() =>
            MediaManager.Pause();
    }
}

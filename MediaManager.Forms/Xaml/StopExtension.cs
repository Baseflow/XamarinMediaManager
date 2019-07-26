using MediaManager.Player;

namespace MediaManager.Forms.Xaml
{
    public class StopExtension : MediaExtensionBase
    {
        public StopExtension()
            : base()
        {
        }

        protected override bool CanExecute() =>
            MediaManager.State == MediaPlayerState.Playing;

        protected override void Execute()
        {
            MediaManager.Stop();
        }
    }
}

using MediaManager.Playback;

namespace MediaManager.Forms.Xaml
{
    public class PauseExtension : MediaExtensionBase
    {
        protected override bool CanExecute() => 
            MediaManager.State == MediaPlayerState.Playing;

        protected override void Execute() => 
            MediaManager.Pause();
    }
}

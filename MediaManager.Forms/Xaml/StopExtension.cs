using MediaManager.Playback;

namespace MediaManager.Forms.Xaml
{
    public class StopExtension : MediaExtensionBase
    {
        protected override bool CanExecute() => 
            MediaManager.State == MediaPlayerState.Playing;

        protected override void Execute()
        {
            MediaManager.Stop();
            MediaManager.PlayPreviousOrSeekToStart();
            MediaManager.SeekToStart();
            MediaManager.StepBackward();
            MediaManager.StepForward();
        }
    }
}

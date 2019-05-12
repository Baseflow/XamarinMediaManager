namespace MediaManager.Forms.Xaml
{
    public class PlayPauseExtension : MediaExtensionBase
    {
        protected override void Execute() => 
            MediaManager.PlayPause();
    }
}

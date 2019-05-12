namespace MediaManager.Forms.Xaml
{
    public class PlayPreviousOrSeekToStartExtension : MediaExtensionBase
    {
        protected override void Execute() =>
            MediaManager.PlayPreviousOrSeekToStart();
    }
}

namespace MediaManager.Forms.Xaml
{
    public class PlayPreviousOrSeekToStartExtension : MediaExtensionBase
    {
        public PlayPreviousOrSeekToStartExtension()
            : base()
        {
        }

        protected override void Execute() =>
            MediaManager.PlayPreviousOrSeekToStart();
    }
}

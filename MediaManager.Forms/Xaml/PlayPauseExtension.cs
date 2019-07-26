namespace MediaManager.Forms.Xaml
{
    public class PlayPauseExtension : MediaExtensionBase
    {
        public PlayPauseExtension()
            : base()
        {
        }

        protected override void Execute() =>
            MediaManager.PlayPause();
    }
}

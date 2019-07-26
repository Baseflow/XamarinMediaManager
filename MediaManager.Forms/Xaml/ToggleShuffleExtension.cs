namespace MediaManager.Forms.Xaml
{
    public class ToggleShuffleExtension : MediaExtensionBase
    {
        public ToggleShuffleExtension()
            : base()
        {
        }

        protected override bool CanExecute() => true;

        protected override void Execute() =>
            MediaManager.ToggleShuffle();
    }
}

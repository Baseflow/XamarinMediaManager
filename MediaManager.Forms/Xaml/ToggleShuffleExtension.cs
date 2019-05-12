namespace MediaManager.Forms.Xaml
{
    public class ToggleShuffleExtension : MediaExtensionBase
    {
        protected override bool CanExecute() => true;

        protected override void Execute() => 
            MediaManager.ToggleShuffle();
    }
}

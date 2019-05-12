namespace MediaManager.Forms.Xaml
{
    public class ToggleRepeatExtension : MediaExtensionBase
    {
        protected override bool CanExecute() => true;

        protected override void Execute() => 
            MediaManager.ToggleRepeat();
    }
}

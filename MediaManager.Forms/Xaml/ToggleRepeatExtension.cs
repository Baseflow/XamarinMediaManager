namespace MediaManager.Forms.Xaml
{
    public class ToggleRepeatExtension : MediaExtensionBase
    {
        public ToggleRepeatExtension()
            : base()
        {
        }

        protected override bool CanExecute() => true;

        protected override void Execute() =>
            MediaManager.ToggleRepeat();
    }
}

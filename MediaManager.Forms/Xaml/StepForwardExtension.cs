namespace MediaManager.Forms.Xaml
{
    public class StepForwardExtension : MediaExtensionBase
    {
        public StepForwardExtension()
            : base()
        {
        }

        protected override void Execute() =>
            MediaManager.StepForward();
    }
}

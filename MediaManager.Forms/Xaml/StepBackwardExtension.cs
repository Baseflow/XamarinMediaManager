namespace MediaManager.Forms.Xaml
{
    public class StepBackwardExtension : MediaExtensionBase
    {
        public StepBackwardExtension()
            : base()
        {
        }

        protected override void Execute() =>
            MediaManager.StepBackward();
    }
}

namespace MediaManager.Forms.Xaml
{
    public class StepForwardExtension : MediaExtensionBase
    {
        protected override void Execute() =>
            MediaManager.StepForward();
    }
}

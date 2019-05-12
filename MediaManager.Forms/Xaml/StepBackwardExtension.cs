namespace MediaManager.Forms.Xaml
{
    public class StepBackwardExtension : MediaExtensionBase
    {
        protected override void Execute() =>
            MediaManager.StepBackward();
    }
}

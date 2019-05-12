namespace MediaManager.Forms.Xaml
{
    public class SeekToStartExtension : MediaExtensionBase
    {
        protected override void Execute() =>
            MediaManager.SeekToStart();
    }
}

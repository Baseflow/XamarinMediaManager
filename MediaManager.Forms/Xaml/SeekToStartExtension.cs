namespace MediaManager.Forms.Xaml
{
    public class SeekToStartExtension : MediaExtensionBase
    {
        public SeekToStartExtension()
            : base()
        {
        }

        protected override void Execute() =>
            MediaManager.SeekToStart();
    }
}

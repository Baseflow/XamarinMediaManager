namespace MediaManager.Forms.Xaml
{
    public class PlayPreviousExtension : MediaExtensionBase
    {
        public PlayPreviousExtension()
            : base()
        {
            MediaManager.Queue.QueueChanged += (s, e) => RaiseCanExecuteChanged();
        }

        protected override bool CanExecute() =>
            MediaManager.Queue.CurrentIndex > 0;

        protected override void Execute() =>
            MediaManager.PlayPrevious();
    }
}

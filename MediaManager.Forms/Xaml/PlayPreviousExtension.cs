namespace MediaManager.Forms.Xaml
{
    public class PlayPreviousExtension : MediaExtensionBase
    {
        public PlayPreviousExtension()
        {
            MediaManager.MediaQueue.CollectionChanged += (s, e) => RaiseCanExecuteChanged();
        }

        protected override bool CanExecute() =>
            MediaManager.MediaQueue.CurrentIndex > 0;

        protected override void Execute() => 
            MediaManager.PlayPrevious();
    }
}

namespace MediaManager.Forms.Xaml
{
    public class PlayNextExtension : MediaExtensionBase
    {
        public PlayNextExtension()
            : base()
        {
            MediaManager.MediaQueue.CollectionChanged += (s, e) => RaiseCanExecuteChanged();
        }

        protected override bool CanExecute() =>
            MediaManager.MediaQueue.CurrentIndex < MediaManager.MediaQueue.Count;

        protected override void Execute() =>
            MediaManager.PlayNext();
    }
}

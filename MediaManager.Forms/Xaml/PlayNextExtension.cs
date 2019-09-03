namespace MediaManager.Forms.Xaml
{
    public class PlayNextExtension : MediaExtensionBase
    {
        public PlayNextExtension()
            : base()
        {
            MediaManager.Queue.CollectionChanged += (s, e) => RaiseCanExecuteChanged();
        }

        protected override bool CanExecute() =>
            MediaManager.Queue.CurrentIndex < MediaManager.Queue.Count;

        protected override void Execute() =>
            MediaManager.PlayNext();
    }
}

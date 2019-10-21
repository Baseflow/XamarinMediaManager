namespace MediaManager.Media
{
    public class MediaExtractorProviderBase : NotifyPropertyChangedBase, IMediaExtractorProvider
    {
        private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }
    }
}

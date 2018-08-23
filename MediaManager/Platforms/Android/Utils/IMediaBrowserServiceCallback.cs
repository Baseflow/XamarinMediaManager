namespace MediaManager.Platforms.Android.Utils
{
    interface IMediaBrowserServiceCallback
    {
        void UpdateCurrentPlaying(int index);

        void UpdateMedia();
    }
}

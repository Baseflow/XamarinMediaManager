using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;

namespace MediaManager.Platforms.Android.MediaSession
{
    public class MediaBrowserSubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
    {
        public MediaBrowserSubscriptionCallback()
        {
        }

        protected MediaBrowserSubscriptionCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public Action<string, IList<MediaBrowserCompat.MediaItem>> OnChildrenLoadedImpl { get; set; }

        public Action<string, IList<MediaBrowserCompat.MediaItem>, Bundle> OnChildrenLoadedBundleImpl { get; set; }

        public Action<string> OnErrorImpl { get; set; }

        public Action<string, Bundle> OnErrorBundleImpl { get; set; }

        public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
        {
            base.OnChildrenLoaded(parentId, children);
            OnChildrenLoadedImpl?.Invoke(parentId, children);
        }

        public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children, Bundle options)
        {
            base.OnChildrenLoaded(parentId, children, options);
            OnChildrenLoadedBundleImpl?.Invoke(parentId, children, options);
        }

        public override void OnError(string id)
        {
            base.OnError(id);
            OnErrorImpl?.Invoke(id);
        }

        public override void OnError(string parentId, Bundle options)
        {
            base.OnError(parentId, options);
            OnErrorBundleImpl?.Invoke(parentId, options);
        }
    }
}

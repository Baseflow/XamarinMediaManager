using System;
using System.Collections.Generic;
using System.Text;
using Android.Support.V4.Media;

namespace MediaManager.Platforms.Android.Audio
{
    public class MediaBrowserSubscriptionCallback : MediaBrowserCompat.SubscriptionCallback
    {
        public Action<string, IList<MediaBrowserCompat.MediaItem>> OnChildrenLoadedImpl { get; set; }

        public Action<string> OnErrorImpl { get; set; }

        public override void OnChildrenLoaded(string parentId, IList<MediaBrowserCompat.MediaItem> children)
        {
            OnChildrenLoadedImpl?.Invoke(parentId, children);
        }

        public override void OnError(string id)
        {
            OnErrorImpl?.Invoke(id);
        }
    }
}

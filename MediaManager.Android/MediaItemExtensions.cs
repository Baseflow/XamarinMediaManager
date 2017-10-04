using System;
using Android.Support.V4.Media;
using Plugin.MediaManager.Abstractions;
using static Android.Support.V4.Media.MediaBrowserCompat;
using static Android.Support.V4.Media.Session.MediaSessionCompat;

namespace Plugin.MediaManager
{
    public static class MediaItemExtensions
    {
        public static MediaItem ToMediaItem(this IMediaItem item)
        {
            

            MediaDescriptionCompat description = new MediaDescriptionCompat.Builder().SetTitle("test").Build();
            var test = new MediaItem(description, MediaItem.FlagPlayable);
            //var test2 = new QueueItem(description);
            return test;
        }
    }
}

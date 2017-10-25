using System;
using Android.Graphics;
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
            //MediaDescriptionCompat description = new MediaDescriptionCompat.Builder().SetTitle("test").Build();
            //var test = new MediaItem(description, MediaItem.FlagPlayable);
            ////var test2 = new QueueItem(description);
            //return test;

            var _builder = new MediaDescriptionCompat.Builder();

            _builder.SetMediaId(new Guid().ToString())
                .SetTitle(item.Metadata.DisplayTitle)
                .SetDescription(item.Metadata.DisplayDescription)
                .SetSubtitle(item.Metadata.DisplaySubtitle)
                .SetIconBitmap(item.Metadata.DisplayIcon as Bitmap)
                .SetMediaUri(Android.Net.Uri.Parse(item.Url));

            if (!string.IsNullOrEmpty(item.Metadata.DisplayIconUri)) _builder.SetIconUri(Android.Net.Uri.Parse(item.Metadata.DisplayIconUri));
            return new MediaItem(_builder.Build(), MediaItem.FlagPlayable);
        }
    }
}

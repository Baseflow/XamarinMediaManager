using Android.Graphics;
using Android.OS;
using Android.Support.V4.Media;

namespace MediaManager.Platforms.Android.Temp
{
    public class Samples
    {
        public class Sample
        {
            public global::Android.Net.Uri uri;
            public string mediaId;
            public string title;
            public string description;
            public int bitmapResource;

            public Sample(string uri, string mediaId, string title, string description, int bitmapResource)
            {
                this.uri = global::Android.Net.Uri.Parse(uri);
                this.mediaId = mediaId;
                this.title = title;
                this.description = description;
                this.bitmapResource = bitmapResource;
            }

            public override string ToString()
            {
                return title;
            }
        }

        public static Sample[] SAMPLES = new Sample[] {
            new Sample(
              "http://storage.googleapis.com/automotive-media/Jazz_In_Paris.mp3",
              "audio_1",
              "Jazz in Paris",
              "Jazz for the masses",
              -1), //Insert drawable here.
            new Sample(
                "http://storage.googleapis.com/automotive-media/The_Messenger.mp3",
                "audio_2",
                "The messenger",
                "Hipster guide to London",
                -1), //Insert drawable here.
            new Sample(
                "http://storage.googleapis.com/automotive-media/Talkies.mp3",
                "audio_3",
                "Talkies",
                "If it talks like a duck and walks like a duck.",
                -1), //Insert drawable here.
        };

        public static MediaDescriptionCompat GetMediaDescription(global::Android.Content.Context context, Sample sample)
        {
            Bundle extras = new Bundle();
            Bitmap bitmap = GetBitmap(context, sample.bitmapResource);

            if (bitmap != null)
            {
                extras.PutParcelable(MediaMetadataCompat.MetadataKeyAlbumArt, bitmap);
                extras.PutParcelable(MediaMetadataCompat.MetadataKeyDisplayIcon, bitmap);
            }

            return new MediaDescriptionCompat.Builder()
                .SetMediaId(sample.mediaId)
                .SetIconBitmap(bitmap)
                .SetTitle(sample.title)
                .SetDescription(sample.description)
                .SetExtras(extras)
                .Build();
        }

        public static Bitmap GetBitmap(global::Android.Content.Context context, int bitmapResource)
        {
            if (bitmapResource == -1)
                return null;

            return ((global::Android.Graphics.Drawables.BitmapDrawable)context.Resources.GetDrawable(bitmapResource)).Bitmap;
        }
    }
}

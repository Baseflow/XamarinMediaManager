using Android.Content;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static void Init(this IMediaManager mediaManager, Context context)
        {
            mediaManager.SetContext(context);
            mediaManager.Init();
        }

        public static void SetContext(this IMediaManager mediaManager, Context context)
        {
            (mediaManager as MediaManagerImplementation).Context = context;
        }

        public static Context GetContext(this IMediaManager mediaManager)
        {
            return (mediaManager as MediaManagerImplementation).Context;
        }
    }
}

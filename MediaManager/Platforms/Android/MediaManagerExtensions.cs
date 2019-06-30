using Android.Content;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static void Init(this IMediaManager mediaManager, Context context)
        {
            ((MediaManagerImplementation)mediaManager).Context = context;
            mediaManager.Init();
        }
    }
}

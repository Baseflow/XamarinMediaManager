using Android.Content;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static void Init(this IMediaManager mediaManager, Context context)
        {
            var androidMediaManager = ((MediaManagerImplementation)mediaManager);
            androidMediaManager.Context = context;
            androidMediaManager.Init();
        }
    }
}

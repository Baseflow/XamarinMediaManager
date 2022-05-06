using Android.Runtime;
using Com.Google.Android.Exoplayer2.Metadata;

namespace MediaManager.Platforms.Android.Player
{
    public class MetadataOutput : Java.Lang.Object, IMetadataOutput
    {
        public MetadataOutput()
        {
        }

        public MetadataOutput(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public void OnMetadata(Metadata metadata)
        {

        }
    }
}

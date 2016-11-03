using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.Android
{
    public class VideoViewRenderer : ViewRenderer<VideoView, VideoSurface>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                SetNativeControl(new VideoSurface(Context));
            }
        }
    }
}

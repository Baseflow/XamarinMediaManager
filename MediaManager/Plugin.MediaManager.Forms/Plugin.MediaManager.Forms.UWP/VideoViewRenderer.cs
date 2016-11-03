using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.UWP
{
    public class VideoViewRenderer : ViewRenderer<VideoView, VideoSurface>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                SetNativeControl(new VideoSurface());
            }
        }
    }
}

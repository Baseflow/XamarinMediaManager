using Plugin.MediaManager.XamarinForms;
using Plugin.MediaManager.XamarinForms.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.XamarinForms.Android
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

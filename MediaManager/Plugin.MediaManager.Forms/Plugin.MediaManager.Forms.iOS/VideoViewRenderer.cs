using Plugin.MediaManager.XamarinForms;
using Plugin.MediaManager.XamarinForms.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.XamarinForms.iOS
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

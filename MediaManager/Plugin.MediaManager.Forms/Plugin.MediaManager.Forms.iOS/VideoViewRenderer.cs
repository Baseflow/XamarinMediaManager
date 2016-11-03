using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.iOS
{
    public class VideoViewRenderer : ViewRenderer<VideoView, VideoSurface> 
    {
        private VideoSurface _videoSurface;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _videoSurface = new VideoSurface();
                SetNativeControl(_videoSurface);
                CrossMediaManager.Current.VideoPlayer.SetVideoSurface(_videoSurface);
            }
        }
    }
}

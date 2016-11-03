using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.UWP;
using Xamarin.Forms.Platform.UWP;
using Size = Windows.Foundation.Size;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.UWP
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
                CrossMediaManager.Current.VideoPlayer.SetAspectMode(VideoAspectMode.AspectFill);
                SetNativeControl(_videoSurface);
                CrossMediaManager.Current.VideoPlayer.SetVideoSurface(_videoSurface);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _videoSurface.Height = availableSize.Height;
            _videoSurface.Width = availableSize.Width;
            return base.MeasureOverride(availableSize);
        }
    }
}

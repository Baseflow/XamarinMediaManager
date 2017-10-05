using Plugin.MediaManager.Abstractions.Enums;
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
                CrossMediaManager.Current.VideoPlayer.AspectMode = Element.AspectMode;
                SetNativeControl(_videoSurface);
                CrossMediaManager.Current.VideoPlayer.RenderSurface = _videoSurface;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.MeasureOverride(availableSize);
            _videoSurface.Height = availableSize.Height;
            _videoSurface.Width = availableSize.Width;
            return availableSize;
        }
    }
}

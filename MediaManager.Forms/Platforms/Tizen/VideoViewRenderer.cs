using MediaManager.Forms.Platforms.Tizen;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Tizen
{
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Tizen.Video.VideoView>
    {
        private MediaManager.Platforms.Tizen.Video.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> args)
        {
            if (args.OldElement != null)
            {
                args.OldElement.Dispose();
            }
            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    _videoView = new MediaManager.Platforms.Tizen.Video.VideoView(NativeView.Parent);
                    SetNativeControl(_videoView);
                }
            }

            base.OnElementChanged(args);
        }

        protected override void UpdateBackgroundColor(bool initialize)
        {
            base.UpdateBackgroundColor(initialize);
            if (Control != null)
                Control.Color = Element.BackgroundColor.ToNative();
        }

        protected override void Dispose(bool disposing)
        {
            _videoView = null;
            base.Dispose(disposing);
        }
    }
}

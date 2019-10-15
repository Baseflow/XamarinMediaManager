using MediaManager.Forms.Platforms.Mac;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Mac
{
    [Foundation.Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Mac.Video.VideoView>
    {
        private MediaManager.Platforms.Mac.Video.VideoView _videoView;

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
                    //TODO: maybe pass in the NSView to the videoview here
                    _videoView = new MediaManager.Platforms.Mac.Video.VideoView();
                    SetNativeControl(_videoView);
                }
            }

            base.OnElementChanged(args);
        }

        protected override void SetBackgroundColor(Color color)
        {
            base.SetBackgroundColor(color);
            if (Control?.Layer != null)
                Control.Layer.BackgroundColor = color.ToCGColor();
        }

        protected override void Dispose(bool disposing)
        {
            _videoView = null;
            base.Dispose(disposing);
        }
    }
}

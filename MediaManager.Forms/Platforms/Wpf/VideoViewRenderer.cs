using MediaManager.Forms.Platforms.Wpf;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Wpf
{
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Wpf.Video.VideoView>
    {
        private MediaManager.Platforms.Wpf.Video.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> args)
        {
            base.OnElementChanged(args);

            if (args.OldElement != null)
            {
                args.OldElement.Dispose();
            }
            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    _videoView = new MediaManager.Platforms.Wpf.Video.VideoView();
                    SetNativeControl(_videoView);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _videoView = null;
            base.Dispose(disposing);
        }
    }
}

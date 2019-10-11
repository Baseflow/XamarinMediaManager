using MediaManager.Forms.Platforms.Uap;
using Windows.Foundation;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Uap
{
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Uap.Video.VideoView>
    {
        private MediaManager.Platforms.Uap.Video.VideoView _videoView;

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
                    _videoView = new MediaManager.Platforms.Uap.Video.VideoView();
                    SetNativeControl(_videoView);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_videoView != null)
            {
                _videoView.Height = availableSize.Height;
                _videoView.Width = availableSize.Width;

                _videoView.PlayerView.Height = availableSize.Height;
                _videoView.PlayerView.Width = availableSize.Width;
            }
            return base.MeasureOverride(availableSize);
        }

        protected override void Dispose(bool disposing)
        {
            _videoView = null;
            base.Dispose(disposing);
        }
    }
}

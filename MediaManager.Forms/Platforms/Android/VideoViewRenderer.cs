using Android.Content;
using MediaManager.Forms;
using MediaManager.Forms.Platforms.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Android
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class VideoViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<VideoView, MediaManager.Platforms.Android.Video.VideoView>
    {
        private MediaManager.Platforms.Android.Video.VideoView _videoView;

        public VideoViewRenderer(Context context) : base(context)
        {
        }

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
                    _videoView = new MediaManager.Platforms.Android.Video.VideoView(Context);

                    //TODO: find a better way to set properties on load
                    _videoView.ShowControls = args.NewElement.ShowControls;
                    _videoView.VideoAspect = args.NewElement.VideoAspect;

                    SetNativeControl(_videoView);
                }
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (_videoView != null)
            {
                var p = _videoView.LayoutParameters;
                p.Height = heightMeasureSpec;
                p.Width = widthMeasureSpec;
                _videoView.LayoutParameters = p;
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void Dispose(bool disposing)
        {
            //On Android we need to call this manually it seems, since args.OldElement is never filled
            Element?.Dispose();

            _videoView = null;
            base.Dispose(disposing);
        }
    }
}

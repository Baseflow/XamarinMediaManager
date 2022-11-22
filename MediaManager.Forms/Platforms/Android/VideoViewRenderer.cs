using Android.Content;
using MediaManager.Forms;
using MediaManager.Forms.Platforms.Android;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Android
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Android.Video.VideoView>
    {
        private MediaManager.Platforms.Android.Video.VideoView _videoView;

        public VideoViewRenderer(Context context) : base(context)
        {
        }

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
                    _videoView = new MediaManager.Platforms.Android.Video.VideoView(Context);
                    SetNativeControl(_videoView);
                    UpdateBackgroundColor();
                    UpdateLayout();
                }
            }

            base.OnElementChanged(args);
        }

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();
            Control?.SetShutterBackgroundColor(Microsoft.Maui.Controls.Compatibility.Platform.Android.ColorExtensions.ToAndroid(Element.BackgroundColor));
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (_videoView != null && widthMeasureSpec > -1 && heightMeasureSpec > -1)
            {
                var p = _videoView.LayoutParameters;

                if (p == null)
                    p = new LayoutParams(widthMeasureSpec, heightMeasureSpec);
                else
                {
                    p.Height = heightMeasureSpec;
                    p.Width = widthMeasureSpec;
                }
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

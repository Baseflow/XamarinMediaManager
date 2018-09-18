using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Forms;
using Xamarin.Forms.Platform.Android;

/*[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Android
{
    public class VideoViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<VideoView, VideoSurface>
    {
        private VideoSurface _videoSurface;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _videoSurface = new VideoSurface(Context);
                SetNativeControl(_videoSurface);
                CrossMediaManager.Current.VideoPlayer.RenderSurface = _videoSurface;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var p = _videoSurface.LayoutParameters;
            p.Height = heightMeasureSpec;
            p.Width = widthMeasureSpec;
            _videoSurface.LayoutParameters = p;
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}*/

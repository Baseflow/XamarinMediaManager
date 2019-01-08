using System;
using Android.Views;
using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.Android
{
    public class VideoViewRenderer : ViewRenderer<VideoView, VideoSurface>
    {
        private VideoSurface _videoSurface;

        public static async void Init()
        {
            var temp = DateTime.Now;
        }
        


        protected override void OnVisibilityChanged(View changedView, ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);

            if (visibility == ViewStates.Visible)
            {
                CrossMediaManager.Current.VideoPlayer.RenderSurface = _videoSurface;
            }
        } 

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _videoSurface = new VideoSurface(Context);
                SetNativeControl(_videoSurface);
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
}

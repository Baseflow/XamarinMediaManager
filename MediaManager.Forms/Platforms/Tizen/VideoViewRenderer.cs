using System;
using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.Tizen;
using Xamarin.Forms.Platform.Tizen;
using TForms = Xamarin.Forms.Platform.Tizen.Forms;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.Tizen
{
    public class VideoViewRenderer : ViewRenderer<VideoView, VideoSurface>
    {
        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            if (Control == null)
            {
                var videoSurface = new VideoSurface(TForms.NativeParent);
                SetNativeControl(videoSurface);
                CrossMediaManager.Current.VideoPlayer.AspectMode = Element.AspectMode;
                CrossMediaManager.Current.VideoPlayer.RenderSurface = videoSurface;
            }
            base.OnElementChanged(e);
        }
    }
}

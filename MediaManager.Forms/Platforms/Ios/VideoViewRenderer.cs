using System;
using Foundation;
using MediaManager;
using MediaManager.Forms;
using MediaManager.Forms.Platforms.iOS;
using MediaManager.Platforms.Ios.Video;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.iOS
{
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Ios.Video.VideoSurface> 
    {
        private MediaManager.Platforms.Ios.Video.VideoSurface _videoSurface;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _videoSurface = new VideoSurface(Control);
                SetNativeControl(_videoSurface);
                CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoSurface);
            }
        }
    }
}

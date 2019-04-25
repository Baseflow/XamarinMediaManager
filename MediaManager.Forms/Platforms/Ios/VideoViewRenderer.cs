using System;
using Foundation;
using MediaManager.Forms.Platforms.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.iOS
{
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Ios.Video.VideoSurface>
    {
        private MediaManager.Platforms.Ios.Video.VideoSurface _videoView;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _videoView = new MediaManager.Platforms.Ios.Video.VideoSurface(Control);
                SetNativeControl(_videoView);
                CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoView);
            }
        }
    }
}

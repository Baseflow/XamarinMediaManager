using System;
using Foundation;
using MediaManager.Forms.Platforms.Mac;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Mac
{
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Mac.Video.VideoSurface>
    {
        private MediaManager.Platforms.Mac.Video.VideoSurface _videoView;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _videoView = new MediaManager.Platforms.Mac.Video.VideoSurface(Control);
                SetNativeControl(_videoView);
                CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoView);
            }
        }
    }
}

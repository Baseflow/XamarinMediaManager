using System;
using Foundation;
using MediaManager;
using MediaManager.Forms;
using MediaManager.Forms.Platforms.iOS;
using MediaManager.Platforms.Ios.Video;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.iOS
{
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Ios.Video.VideoView> 
    {
        private MediaManager.Platforms.Ios.Video.VideoView _videoView;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _videoView = new MediaManager.Platforms.Ios.Video.VideoView(Control);
                SetNativeControl(_videoView);
                CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoView);
            }
        }
    }
}

using System;
using Foundation;
using MediaManager.Forms.Platforms.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.iOS
{
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, UIView>
    {
        private MediaManager.Platforms.Ios.Video.VideoView _videoView;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _videoView = new MediaManager.Platforms.Ios.Video.VideoView();

                    SetNativeControl(_videoView.View);
                    CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoView);
                }
            }
        }
    }
}

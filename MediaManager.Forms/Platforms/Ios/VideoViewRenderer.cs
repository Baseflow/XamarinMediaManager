using System;
using Foundation;
using MediaManager.Forms.Platforms.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.iOS
{
    [Foundation.Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, UIView>
    {
        private MediaManager.Platforms.Ios.Video.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    //TODO: maybe pass in the UIView to the videoview here
                    _videoView = new MediaManager.Platforms.Ios.Video.VideoView();
                    CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoView);
                    SetNativeControl(_videoView);
                }
            }
        }
    }
}

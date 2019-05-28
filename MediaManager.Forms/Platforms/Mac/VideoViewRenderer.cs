using System;
using Foundation;
using MediaManager.Forms.Platforms.Mac;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Mac
{
    [Foundation.Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Mac.Video.VideoView>
    {
        private MediaManager.Platforms.Mac.Video.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> args)
        {
            base.OnElementChanged(args);
            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    //TODO: maybe pass in the NSView to the videoview here
                    _videoView = new MediaManager.Platforms.Mac.Video.VideoView();

                    //TODO: find a better way to set properties on load
                    _videoView.ShowControls = args.NewElement.ShowControls;
                    _videoView.VideoAspect = args.NewElement.VideoAspect;

                    CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoView);
                    SetNativeControl(_videoView);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _videoView = null;
            base.Dispose(disposing);
        }
    }
}

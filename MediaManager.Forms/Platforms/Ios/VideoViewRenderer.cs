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

        protected override async void OnElementChanged(ElementChangedEventArgs<VideoView> args)
        {
            base.OnElementChanged(args);

            if (args.OldElement != null)
            {
                args.OldElement.Dispose();
            }
            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    //TODO: maybe pass in the UIView to the videoview here
                    _videoView = new MediaManager.Platforms.Ios.Video.VideoView();
                    if(args.NewElement.VideoPlaceholder != null)
                    {
                        _videoView.VideoPlaceholder = await args.NewElement.VideoPlaceholder.ToNative();
                    }
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

using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Forms.Platforms.Uap;
using MediaManager.Platforms.Uap.Video;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Uap
{
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Uap.Video.VideoView>
    {
        private MediaManager.Platforms.Uap.Video.VideoView _videoView;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> args)
        {
            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    _videoView = new MediaManager.Platforms.Uap.Video.VideoView();

                    //TODO: find a better way to set properties on load
                    _videoView.ShowControls = args.NewElement.ShowControls;
                    _videoView.VideoAspect = args.NewElement.VideoAspect;

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

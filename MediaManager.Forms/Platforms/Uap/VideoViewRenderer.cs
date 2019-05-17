using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Forms.Platforms.Uap;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(MediaManager.Forms.VideoView), typeof(VideoViewRenderer))]
namespace MediaManager.Forms.Platforms.Uap
{
    public class VideoViewRenderer : ViewRenderer<VideoView, MediaManager.Platforms.Uap.Video.VideoView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> args)
        {
            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    var videoView = new MediaManager.Platforms.Uap.Video.VideoView();
                    SetNativeControl(videoView);
                }
            }
        }
    }
}

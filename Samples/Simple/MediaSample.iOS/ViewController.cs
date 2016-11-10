using System;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.iOS;
using UIKit;

namespace MediaSample.iOS
{
    public partial class ViewController : UIViewController
    {
		VideoSurface _videoSurface;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			_videoSurface = new VideoSurface();
			VideoView.Add(_videoSurface);
			CrossMediaManager.Current.VideoPlayer.SetVideoSurface(_videoSurface);
			CrossMediaManager.Current.PlayingChanged += (sender, e) => ProgressView.Progress = (float)e.Progress;

            // Perform any additional setup after loading the view, typically from a nib.
        }

		public override void ViewDidLayoutSubviews()
		{
			_videoSurface.Frame = VideoView.Frame;
			base.ViewDidLayoutSubviews();
		}

		partial void PlayButton_TouchUpInside(UIButton sender)
		{
			CrossMediaManager.Current.Play("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4", MediaFileType.VideoUrl);
		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MediaSample.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UISlider BufferedProgressSlider { get; set; }

		[Outlet]
		UIKit.UIButton NextButton { get; set; }

		[Outlet]
		UIKit.UISlider PlayingProgressSlider { get; set; }

		[Outlet]
		UIKit.UIButton PlayPauseButton { get; set; }

		[Outlet]
		UIKit.UIButton PreviousButton { get; set; }

		[Outlet]
		UIKit.UILabel QueueLabel { get; set; }

		[Outlet]
		UIKit.UIButton RepeatButton { get; set; }

		[Outlet]
		UIKit.UIButton ShuffleButton { get; set; }

		[Outlet]
		UIKit.UILabel TimePlayedLabel { get; set; }

		[Outlet]
		UIKit.UILabel TimeRemainingLabel { get; set; }

		[Outlet]
		UIKit.UIImageView TrackCoverImageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (QueueLabel != null) {
				QueueLabel.Dispose ();
				QueueLabel = null;
			}

			if (TrackCoverImageView != null) {
				TrackCoverImageView.Dispose ();
				TrackCoverImageView = null;
			}

			if (BufferedProgressSlider != null) {
				BufferedProgressSlider.Dispose ();
				BufferedProgressSlider = null;
			}

			if (PlayingProgressSlider != null) {
				PlayingProgressSlider.Dispose ();
				PlayingProgressSlider = null;
			}

			if (TimePlayedLabel != null) {
				TimePlayedLabel.Dispose ();
				TimePlayedLabel = null;
			}

			if (TimeRemainingLabel != null) {
				TimeRemainingLabel.Dispose ();
				TimeRemainingLabel = null;
			}

			if (PlayPauseButton != null) {
				PlayPauseButton.Dispose ();
				PlayPauseButton = null;
			}

			if (NextButton != null) {
				NextButton.Dispose ();
				NextButton = null;
			}

			if (PreviousButton != null) {
				PreviousButton.Dispose ();
				PreviousButton = null;
			}

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}

			if (RepeatButton != null) {
				RepeatButton.Dispose ();
				RepeatButton = null;
			}
		}
	}
}

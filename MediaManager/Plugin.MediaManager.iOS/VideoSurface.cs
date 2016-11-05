using AVFoundation;
using Plugin.MediaManager.Abstractions;
using UIKit;

namespace Plugin.MediaManager.iOS
{
    public class VideoSurface : UIView, IVideoSurface
    {

		public override void LayoutSubviews()
		{
			foreach (var layer in Layer.Sublayers)
			{
				var avPlayerLayer = layer as AVPlayerLayer;
				if (avPlayerLayer != null)
					avPlayerLayer.Frame = Bounds;
			}
			base.LayoutSubviews();
		}

    }
}

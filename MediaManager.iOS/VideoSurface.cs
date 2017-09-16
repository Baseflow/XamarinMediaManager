using System;
using AVFoundation;
using CoreGraphics;
using Plugin.MediaManager.Abstractions;
using UIKit;

namespace Plugin.MediaManager
{
    public class VideoSurface : UIView, IVideoSurface
    {

		public override void LayoutSubviews()
		{
		    Console.WriteLine($"LayoutSubViews requested: {this}");

            base.LayoutSubviews();
			if (Layer.Sublayers == null || Layer.Sublayers.Length == 0)
				return;
			foreach (var layer in Layer.Sublayers)
			{
				var avPlayerLayer = layer as AVPlayerLayer;
				if (avPlayerLayer != null)
					avPlayerLayer.Frame = Bounds;
			}
		}
        
        public override CGRect Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                if (value != base.Bounds)
                {                    
                    Console.WriteLine($"Bounds Width: {value.Width}, Height: {value.Height}");
                }
                base.Bounds = value;
            }
        }

        public override CGRect Frame
        {
            get
            {
                return base.Frame;
            }
            set
            {
                if (value != base.Frame)
                {                    
                    Console.WriteLine($"Frame Width: {value.Width}, Height: {value.Height}");
                }                
                base.Frame = value;
            }
        }        

        #region IDisposable
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;

            base.Dispose(disposing);
        }
        #endregion
    }
}

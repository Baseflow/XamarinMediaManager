using AVFoundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    public class VideoSurface : UIView, IVideoView
    {

        public override void LayoutSubviews()
        {
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

        #region IDisposable
        public bool IsDisposed { get; private set; }
        public VideoAspectMode VideoAspect { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;

            base.Dispose(disposing);
        }
        #endregion
    }
}

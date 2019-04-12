using AVFoundation;
using CoreGraphics;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    public class VideoView : UIView, IVideoView
    {
        private UIView _view;

        public VideoView(UIView view)
        {
            this._view = view;
            Frame = view.Frame;
            view.Add(this);
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

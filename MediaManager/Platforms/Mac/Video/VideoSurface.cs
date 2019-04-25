using AppKit;
using MediaManager.Video;

namespace MediaManager.Platforms.Mac.Video
{
    public class VideoSurface : NSView, IVideoView
    {
        private NSView _view;

        public VideoSurface(NSView view)
        {
            this._view = view;
            Frame = view.Frame;
            view.AddSubview(this);
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

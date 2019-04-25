using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Tvos.Video
{
    public class VideoSurface : UIView, IVideoView
    {
        private UIView _view;

        public VideoSurface(UIView view)
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

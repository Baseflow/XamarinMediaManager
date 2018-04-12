using Plugin.MediaManager.Abstractions;
using UIKit;

namespace Plugin.MediaManager
{
    public class VideoSurface : UIView, IVideoSurface
    {
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

using Plugin.MediaManager.Abstractions;
using AppKit;

namespace Plugin.MediaManager
{
	public class VideoSurface : NSView, IVideoSurface
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

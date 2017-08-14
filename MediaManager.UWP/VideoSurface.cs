using System;
using Windows.UI.Xaml.Controls;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
    public class VideoSurface : Canvas, IVideoSurface, IDisposable
    {
        #region IDisposable
        public bool IsDisposed => disposed;

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.                
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~VideoSurface()
        {
            Dispose(false);
        }
        #endregion        
    }
}

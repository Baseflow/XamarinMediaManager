using System;
using ElmSharp;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Tizen.Multimedia;

namespace Plugin.MediaManager
{
    public class VideoSurface : Box, IVideoSurface, IDisposable
    {
        public MediaView MediaView { get; set; } = null;

        public Player Player { get; internal set; } = null;

        public VideoSurface(EvasObject parent) : base(parent)
        {
            var mediaView = new MediaView(parent)
            {
                Geometry = Geometry,
            };
            SetMediaView(mediaView);
            SetLayoutCallback(OnLayoutUpdated);
            Show();
        }

        public void SetMediaView(MediaView view)
        {
            UnPackAll();
            MediaView = view;
            if (MediaView != null)
            {
                MediaView.Show();
                PackEnd(view);
            }
        }

        void OnLayoutUpdated()
        {
            if (MediaView != null)
            {
                MediaView.Geometry = new Rect(Geometry.X, Geometry.Y, Geometry.Width, Geometry.Height);
            }
        }

        protected override void OnUnrealize()
        {
            SetLayoutCallback(null);
            if (Player?.State != PlayerState.Idle)
            {
                if (CrossMediaManager.Current.VideoPlayer.Status == MediaPlayerStatus.Playing)
                {
                    CrossMediaManager.Current.VideoPlayer.Stop();
                }
                Player.Unprepare();
            }
            base.OnUnrealize();
        }

        #region IDisposable
        public bool IsDisposed => disposed;
        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (MediaView != null)
                {
                    MediaView.Unrealize();
                    MediaView = null;
                }
            }

            disposed = true;
        }

        ~VideoSurface()
        {
            Dispose(false);
        }
        #endregion
    }
}

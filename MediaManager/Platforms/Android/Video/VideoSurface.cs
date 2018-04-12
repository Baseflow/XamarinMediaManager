using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager
{
	[Register("plugin.mediamanager.VideoSurface")]
    public class VideoSurface : VideoView, IVideoSurface
    {
        public VideoSurface(Context context) : base(context)
        {
        }

		public VideoSurface(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public VideoSurface(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}

		public VideoSurface(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
		{
		}

		protected VideoSurface (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
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

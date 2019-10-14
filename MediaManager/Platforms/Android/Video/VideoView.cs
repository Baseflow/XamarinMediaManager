using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Video;

namespace MediaManager.Platforms.Android.Video
{
    [Register("mediamanager.platforms.android.video.VideoView")]
    public class VideoView : PlayerView, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        public VideoView(Context context) : base(context)
        {
            InitView();
        }

        public VideoView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitView();
        }

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitView();
        }

        protected VideoView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public virtual void InitView()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView)
                MediaManager.MediaPlayer.VideoView = this;
        }


        protected override void Dispose(bool disposing)
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView && MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;

            base.Dispose(disposing);
        }
    }
}

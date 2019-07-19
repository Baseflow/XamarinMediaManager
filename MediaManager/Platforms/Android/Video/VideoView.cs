using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Com.Google.Android.Exoplayer2;
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

        public VideoAspectMode VideoAspect
        {
            get
            {
                switch (ResizeMode)
                {
                    case AspectRatioFrameLayout.ResizeModeFill:
                        return VideoAspectMode.AspectFill;
                    case AspectRatioFrameLayout.ResizeModeFit:
                        return VideoAspectMode.AspectFit;
                    case AspectRatioFrameLayout.ResizeModeFixedHeight:
                        return VideoAspectMode.AspectFill;
                    case AspectRatioFrameLayout.ResizeModeFixedWidth:
                        return VideoAspectMode.AspectFill;
                    case AspectRatioFrameLayout.ResizeModeZoom:
                        return VideoAspectMode.None;
                    default:
                        return VideoAspectMode.None;
                }
            }
            set
            {
                switch (value)
                {
                    case VideoAspectMode.None:
                        ResizeMode = AspectRatioFrameLayout.ResizeModeZoom;
                        break;
                    case VideoAspectMode.AspectFit:
                        ResizeMode = AspectRatioFrameLayout.ResizeModeFit;
                        break;
                    case VideoAspectMode.AspectFill:
                        ResizeMode = AspectRatioFrameLayout.ResizeModeFill;
                        break;
                    default:
                        ResizeMode = AspectRatioFrameLayout.ResizeModeZoom;
                        break;
                }
            }
        }

        public bool ShowControls {
            get => UseController;
            set => UseController = value;
        }

        protected override void Dispose(bool disposing)
        {
            if(MediaManager.MediaPlayer.VideoView == this)
                MediaManager.MediaPlayer.VideoView = null;

            base.Dispose(disposing);
        }
    }
}

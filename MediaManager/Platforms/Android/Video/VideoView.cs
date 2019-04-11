using System;
using System.Collections.Generic;
using System.Text;
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
        public VideoView(Context context) : base(context)
        {
        }

        public VideoView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public VideoView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        protected VideoView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public VideoAspectMode VideoAspect {
            get
            {
                switch (VideoAspect)
                {
                    case VideoAspectMode.None:
                        return VideoAspectMode.None;
                    case VideoAspectMode.AspectFit:
                        return VideoAspectMode.AspectFit;
                    case VideoAspectMode.AspectFill:
                        return VideoAspectMode.AspectFill;
                    default:
                        return VideoAspectMode.None;
                }
            }
            set
            {
                switch (value)
                {
                    case VideoAspectMode.None:
                        VideoAspect = VideoAspectMode.None;
                        break;
                    case VideoAspectMode.AspectFit:
                        VideoAspect = VideoAspectMode.AspectFit;
                        break;
                    case VideoAspectMode.AspectFill:
                        VideoAspect = VideoAspectMode.AspectFill;
                        break;
                    default:
                        VideoAspect = VideoAspectMode.None;
                        break;
                }
            }
        }
    }
}

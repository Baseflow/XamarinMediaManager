using ElmSharp;
using MediaManager.Video;
using Tizen.Multimedia;

namespace MediaManager.Platforms.Tizen.Video
{
    public class VideoView : MediaView, IVideoView
    {
        public VideoView(EvasObject parent) : base(parent)
        {
        }

        public VideoAspectMode VideoAspect { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool ShowControls { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}

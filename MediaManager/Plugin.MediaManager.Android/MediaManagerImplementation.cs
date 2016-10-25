using Plugin.MediaManager.Abstractions;
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions.Implementations;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : MediaManagerBase
    {
        public MediaManagerImplementation()
        {
            Init();
        }
        public override IAudioPlayer AudioPlayer { get; set; } = new AudioPlayerImplementation();
        public override IVideoPlayer VideoPlayer { get; set; } = new VideoPlayerImplementation();
        public override IMediaNotificationManager MediaNotificationManager { get; set; }
        public override IMediaExtractor MediaExtractor { get; set; }
    }
}
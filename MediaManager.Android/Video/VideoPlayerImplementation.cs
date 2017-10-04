using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.Lang;
using Java.Util.Concurrent;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : Java.Lang.Object,
        IVideoPlayer
    {
        private MediaManagerImplementation mediaManagerImplementation;

        public VideoPlayerImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public IVideoSurface RenderSurface { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsReadyRendering => throw new NotImplementedException();

        public VideoAspectMode AspectMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PlaybackState State => throw new NotImplementedException();

        public TimeSpan Position => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public TimeSpan Buffered => throw new NotImplementedException();

        public Dictionary<string, string> RequestHeaders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event StatusChangedEventHandler Status;
        public event PlayingChangedEventHandler Playing;
        public event BufferingChangedEventHandler Buffering;
        public event MediaFinishedEventHandler Finished;
        public event MediaFailedEventHandler Failed;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(string url)
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaItem item)
        {
            throw new NotImplementedException();
        }

        public Task Seek(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}

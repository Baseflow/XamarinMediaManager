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
        private bool _IsMuted = false;
        public bool IsMuted
        {
            get { return _IsMuted; }
            set
            {
                if (_IsMuted == value)
                    return;

                int volumeValue = 0;
                if (!value)
                {
                    //https://developer.xamarin.com/api/member/Android.Media.AudioManager.GetStreamVolume/p/Android.Media.Stream/
                    //https://stackoverflow.com/questions/17898382/audiomanager-getstreamvolumeaudiomanager-stream-music-returns-0
                    Stream streamType = Stream.Music;
                    int volumeMax = mediaManagerImplementation.VolumeManager.MaxVolume;
                    int volume = mediaManagerImplementation.VolumeManager.CurrentVolume;

                    //ltang: Unmute with the current volume
                    volumeValue = volume / volumeMax;
                }

                SetVolume(volumeValue);
                _IsMuted = value;
            }
        }

        public void SetVolume(int newVolume)
        {
            try
            {
                mediaManagerImplementation.VolumeManager.CurrentVolume = newVolume;
            }
            catch (Java.Lang.IllegalStateException e)
            {
                //ltang: Wrong state to set volume
                throw;
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

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

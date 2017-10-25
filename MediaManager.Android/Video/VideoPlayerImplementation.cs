using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
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
        private MediaManagerImplementation _mediaManagerImplementation;

        public VideoPlayerImplementation(MediaManagerImplementation mediaManagerImplementation)
        {
            this._mediaManagerImplementation = mediaManagerImplementation;
        }

        public IVideoSurface RenderSurface { get; set; } 

        public bool IsReadyRendering { get; private set; }

        public VideoAspectMode AspectMode { get; set; } = VideoAspectMode.None;
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
                    int volumeMax = _mediaManagerImplementation.VolumeManager.MaxVolume;
                    int volume = _mediaManagerImplementation.VolumeManager.CurrentVolume;

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
                _mediaManagerImplementation.VolumeManager.CurrentVolume = newVolume;
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

        public PlaybackState State => _mediaManagerImplementation.State;

        public TimeSpan Position => _mediaManagerImplementation.Position;

        public TimeSpan Duration => _mediaManagerImplementation.Duration;

        public TimeSpan Buffered => _mediaManagerImplementation.Buffered;

        public Dictionary<string, string> RequestHeaders { get => _mediaManagerImplementation.RequestHeaders; set => _mediaManagerImplementation.RequestHeaders = value; }

        public event StatusChangedEventHandler Status;
        public event PlayingChangedEventHandler Playing;
        public event BufferingChangedEventHandler Buffering;
        public event MediaFinishedEventHandler Finished;
        public event MediaFailedEventHandler Failed;

        public Task Pause()
        {
            return _mediaManagerImplementation?.Pause();
        }

        public Task Play(string url)
        {
            return _mediaManagerImplementation?.Play(url);
        }

        public Task Play(IMediaItem item)
        {
            return _mediaManagerImplementation?.Play(item);
        }

        public Task Seek(TimeSpan position)
        {
            return _mediaManagerImplementation?.Seek(position);
        }

        public Task Stop()
        {
            return _mediaManagerImplementation?.Stop();
        }
    }
}

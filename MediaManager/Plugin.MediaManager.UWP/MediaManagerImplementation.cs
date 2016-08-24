using Plugin.MediaManager.Abstractions;
using System;
using System.Threading.Tasks;

namespace Plugin.MediaManager
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class MediaManagerImplementation : IMediaManager
    {
        public int Buffered
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object Cover
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Duration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Position
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public PlayerStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event BufferingEventHandler Buffering;
        public event CoverReloadedEventHandler CoverReloaded;
        public event PlayingEventHandler Playing;
        public event StatusChangedEventHandler StatusChanged;
        public event TrackFinishedEventHandler TrackFinished;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(string url)
        {
            throw new NotImplementedException();
        }

        public Task PlayNext(string url)
        {
            throw new NotImplementedException();
        }

        public Task PlayPause()
        {
            throw new NotImplementedException();
        }

        public Task PlayPrevious(string url)
        {
            throw new NotImplementedException();
        }

        public Task Seek(int position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
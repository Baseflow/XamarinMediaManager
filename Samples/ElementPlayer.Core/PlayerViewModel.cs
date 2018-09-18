using System;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Media;

namespace ElementPlayer.Core
{
    public class PlayerViewModel
    {
        public IMediaManager MediaManager;

        public PlayerViewModel()
        {
            MediaManager = CrossMediaManager.Current;
        }

        public IMediaQueue MediaQueue
        {
            get
            {
                return MediaManager.MediaQueue;
            }
        }
    }
}



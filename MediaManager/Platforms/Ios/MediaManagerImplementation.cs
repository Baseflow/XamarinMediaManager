using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    [Foundation.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : AppleMediaManagerBase<MediaManager.Platforms.Ios.Media.MediaPlayer>
    {
        public MediaManagerImplementation()
        {

        }
    }
}

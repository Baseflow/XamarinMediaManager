using System;
using System.Collections.Generic;
using System.Text;
using AppKit;
using AVFoundation;
using AVKit;
using Foundation;
using MediaManager.Platforms.Apple.Media;

namespace MediaManager.Platforms.Mac.Media
{
    public class MediaPlayer : AppleMediaPlayer
    {
        //TODO: Make possible to hook into
        NSViewController aVPlayerViewController;
    }
}

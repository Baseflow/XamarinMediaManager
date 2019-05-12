using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MediaManager.Forms.Xaml
{

    public class PlayExtension : MediaExtensionBase
    {
        protected override bool CanExecute()
        {
            return MediaManager.State == MediaPlayerState.Paused ||
                MediaManager.State == MediaPlayerState.Stopped;
        }

        protected override void Execute() => 
            MediaManager.Play();
    }
}

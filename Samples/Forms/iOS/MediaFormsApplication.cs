using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Plugin.MediaManager;
using UIKit;

namespace MediaForms.iOS
{
    [Register(Application.AppName)]
    public class MediaFormsApplication : UIApplication
    {
        private MediaManagerImplementation MediaManager => (MediaManagerImplementation)CrossMediaManager.Current;

        public override void RemoteControlReceived(UIEvent uiEvent)
        {
            MediaManager.MediaRemoteControl.RemoteControlReceived(uiEvent);
        }

    }


}
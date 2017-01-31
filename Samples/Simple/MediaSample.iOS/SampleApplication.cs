using Foundation;
using Plugin.MediaManager;
using UIKit;

namespace MediaSample.iOS
{
    [Register("SampleApplication")]
    public class SampleApplication: UIApplication
    {
        private MediaManagerImplementation MediaManager => (MediaManagerImplementation) CrossMediaManager.Current;

        public override void RemoteControlReceived(UIEvent uiEvent)
        {
            MediaManager.MediaRemoteControl.RemoteControlReceived(uiEvent);
        }
    }
}
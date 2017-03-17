using UIKit;

namespace Plugin.MediaManager
{
    public interface IMediaRemoteControl
    {
        void RemoteControlReceived(UIEvent uiEvent);
    }
}
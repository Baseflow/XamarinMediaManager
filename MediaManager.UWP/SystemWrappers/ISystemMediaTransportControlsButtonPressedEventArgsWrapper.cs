using Windows.Media;

namespace Plugin.MediaManager.SystemWrappers
{
    public interface ISystemMediaTransportControlsButtonPressedEventArgsWrapper
    {
        SystemMediaTransportControlsButton Button { get; }
    }
}
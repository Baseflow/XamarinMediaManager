using Windows.Media;

namespace Plugin.MediaManager.SystemWrappers
{
    internal interface ISystemMediaTransportControlsButtonPressedEventArgsWrapper
    {
        SystemMediaTransportControlsButton Button { get; }
    }
}
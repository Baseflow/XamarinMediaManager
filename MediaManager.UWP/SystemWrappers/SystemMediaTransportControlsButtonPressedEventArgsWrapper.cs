using Windows.Media;

namespace Plugin.MediaManager.SystemWrappers
{
    internal class SystemMediaTransportControlsButtonPressedEventArgsWrapper : ISystemMediaTransportControlsButtonPressedEventArgsWrapper
    {
        public SystemMediaTransportControlsButton Button { get; }

        public SystemMediaTransportControlsButtonPressedEventArgsWrapper(SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            Button = args.Button;
        }
    }
}
using Windows.Media;

namespace Plugin.MediaManager.SystemWrappers
{
    class SystemMediaTransportControlsButtonPressedEventArgsWrapper : ISystemMediaTransportControlsButtonPressedEventArgsWrapper
    {
        public SystemMediaTransportControlsButton Button { get; }

        public SystemMediaTransportControlsButtonPressedEventArgsWrapper(SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            Button = args.Button;
        }
    }
}